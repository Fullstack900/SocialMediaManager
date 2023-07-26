import { AxiosRequestConfig, AxiosResponse } from 'axios';
import { API } from './config';
import { Ref } from 'vue';

export const withToken = (token: string | Ref<string>, config: AxiosRequestConfig = {}): AxiosRequestConfig => {
  return {
    ...config,
    headers: {
      ...(config.headers || {}),
      Authorization: `Bearer ${token}`,
      "Access-Control-Allow-Origin": 'Accept'
    },
  };
};

export const simpleGetRequest =
  (url: string) =>
  (config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.get(url, config);

export const simplePostRequest =
  (url: string) =>
  (requestPayload: unknown, config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.post(url, requestPayload, config);

export const simpleHeadRequest =
  (url: string) =>
  (config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.head(url, config);

export const simplePutRequest =
  (url: string) =>
  (requestPayload: unknown, config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.put(url, requestPayload, config);

export const simpleDeleteRequest =
  (url: string) =>
  (config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.delete(url, config);

export const authorizedGetRequest =
  (url: string) =>
  (token: string) =>
  (config: AxiosRequestConfig = {}): Promise<AxiosResponse> => {
    return API.get(url ,withToken(token, config));
  };

export const authorizedPostRequest =
  (url: string) =>
  (token: string) =>
  (requestPayload: unknown, config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.post(url, requestPayload, withToken(token, config));

export const authorizedPatchRequest =
  (url: string) =>
  (token: string) =>
  (requestPayload: unknown, config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.patch(url, requestPayload, withToken(token, config));

export const authorizedPutRequest =
  (url: string) =>
  (token: string) =>
  (requestPayload: unknown, config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.put(url, requestPayload, withToken(token, config));

export const authorizedDeleteRequest =
  (url: string) =>
  (token: string) =>
  (config: AxiosRequestConfig = {}): Promise<AxiosResponse> =>
    API.delete(url, withToken(token, config));
