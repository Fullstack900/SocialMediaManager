import axios from 'axios';
import config from '@/api-config.json';
import qs from 'qs';
import buildQuery from 'odata-query';
import { onUnathorized, onForbidden } from '@/features/auth/apiInterceptors/catchUnauthorized';
import { checkToken } from '@/features/auth/apiInterceptors/initTokenRefresh';
import { onValidationError } from '@/api/interceptors/catchValidationError';
const paramsSerializer = (params: any) =>
  params?.oData
    ? buildQuery(params.oData).slice(ONE)
    : qs.stringify(params, { arrayFormat: 'repeat', skipNulls: true });

export const API = axios.create({
  baseURL: config.IsdevServerBaseUrl ? `${config.devServerBaseUrl}:7127` : config.baseUrl,
  paramsSerializer,
});

API.interceptors.request.use(checkToken, undefined);
API.interceptors.response.use(undefined, onUnathorized);
API.interceptors.response.use(undefined, onForbidden);
API.interceptors.response.use(undefined, onValidationError);
