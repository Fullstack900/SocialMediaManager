import { AxiosError, AxiosResponse } from 'axios';
import { FieldValidationErrorModel } from '@/models/FieldValidationErrorModel';
import { isSignInUrl } from '@/features/auth/endpoints/authEndpointsUtils';

const BAD_REQUEST_STATUS_CODE = 400;

export const onValidationError = async (error: AxiosError): Promise<void | AxiosResponse<unknown>> => {
  if (error.isAxiosError && error.response) {
    if (error.response.status === BAD_REQUEST_STATUS_CODE) {
      const data = error.response.data as any;
      if (data.fields) {
        const result: any = {};
        Object.keys(data.fields).forEach((fieldName) => {
          const messages: string[] = [];
          const validationErrorModels = data.fields[fieldName] as FieldValidationErrorModel[];
          validationErrorModels.forEach((model) => {
            const key = `web.validation_error_messages.${model.key}`;
            let message = '';
                          message = model.default_message ?? '';
                        messages.push(message);
          });
          result[fieldName] = messages;
        });
        throw { fields: result };
      } else if (data.key && data.default_message) {
        const key = `web.validation_error_messages.${data.key}`;
        let message = '';
        if (isSignInUrl(error.config.url as string) && data.key === 'user_not_verified') {
          message = data.key;
        }  else {
          message = data.default_message;
        }
        throw message;
      } else if (typeof data === 'string') {
        //Unknown error type
        throw data;
      } else {
        throw error;
      }
    } else {
      throw error;
    }
  } else {
    throw error;
  }
};
