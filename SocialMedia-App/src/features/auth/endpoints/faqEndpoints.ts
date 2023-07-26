import {
  authorizedGetRequest,
  authorizedPostRequest,
  authorizedPatchRequest,
  authorizedDeleteRequest,
  authorizedPutRequest,
  simpleGetRequest,
} from '@/api/requests';
import {
  FAQ_GET_LIST_API_URL,
  FAQ_GET_API_URL,
  FAQ_POST_API_URL,
  FAQINSTANCE_UPDATE_API_URL,
  FAQINSTANCE_NAMES_API_URL,
  FAQ_LIST_GET_API,
  FAQ_GET_ASSIGN_API,
  FAQ_GET_ASSIGNEDFAQPAGES_API,
} from './faqEndpointsConstants';
import { AxiosRequestConfig, AxiosResponse } from 'axios';

export default {
  getFAQListContent: authorizedGetRequest(`${FAQ_GET_LIST_API_URL}`),
// start
  getFaqList: authorizedGetRequest(`${FAQ_LIST_GET_API}`),

  getFAQAssign: (
    Id: string
  ): ((token: string) => (config?: AxiosRequestConfig<any>) => Promise<AxiosResponse<unknown, any>>) =>
    authorizedGetRequest(`${FAQ_GET_ASSIGN_API}/${Id}/details`),

  getFAQAssignPages: authorizedGetRequest(`${FAQ_GET_ASSIGNEDFAQPAGES_API}`),
// end
  getFAQContent: (
    id: string
  ): ((token: string) => (config?: AxiosRequestConfig<any>) => Promise<AxiosResponse<unknown, any>>) =>
    authorizedGetRequest(`${FAQ_GET_API_URL}/${id}/details`),

  deleteFAQContent: (
    id: string
  ): ((token: string) => (config?: AxiosRequestConfig) => Promise<AxiosResponse<unknown>>) =>
    authorizedDeleteRequest(`${FAQ_GET_API_URL}/${id}`),
  createFAQ: authorizedPostRequest(`${FAQ_POST_API_URL}`),
  updateFAQ: (
    id: string
  ): ((
    token: string
  ) => (requestPayload: unknown, config?: AxiosRequestConfig<any>) => Promise<AxiosResponse<unknown, any>>) =>
    authorizedPatchRequest(`${FAQ_GET_API_URL}/${id}`),

  updateFAQInstances: (
    id: string
  ): ((
    token: string
  ) => (requestPayload: unknown, config?: AxiosRequestConfig<any>) => Promise<AxiosResponse<unknown, any>>) =>
    authorizedPutRequest(`${FAQINSTANCE_UPDATE_API_URL}/${id}`),

  // getFAQInstances: (pageName: string): ((config?: AxiosRequestConfig<any>) => Promise<AxiosResponse<unknown, any>>) =>
  //   simpleGetRequest(`${FAQINSTANCE_UPDATE_API_URL}/${pageName}`),
  getFAQInstances: authorizedPostRequest(`${FAQINSTANCE_NAMES_API_URL}`),
};
