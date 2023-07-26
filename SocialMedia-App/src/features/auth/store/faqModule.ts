import { faq } from '@endpoints/endpoints';
import FAQ from '../models/FAQ';
import FAQPage from '../models/FAQPage';
import FAQASSIGN from '../models/FAQAssign';
import { Module } from 'vuex';
import { ODataResponse } from '@/models/ODataResponse';
import { AxiosResponse } from 'axios';
import Question from '../models/Question';

interface updateFAQData {
  faqId: string;
  delta: any;
}

interface updateFAQInstanceData {
  faqId: string;
  list: any;
}

const module: Module<any, never> = {
  actions: {
    async createFAQ({ rootGetters }, data: FAQ): Promise<void> {
      await faq.createFAQ(rootGetters['auth/accessToken'])(data);
    },
    async updateFAQ({ rootGetters }, data: updateFAQData): Promise<void> {
      await faq.updateFAQ(data.faqId)(rootGetters['auth/accessToken'])(data.delta);
    },
    async getFAQs({ rootGetters }, params) {
      const accessToken = rootGetters['auth/accessToken'];
      const { data } = (await faq.getFAQListContent(accessToken)({ params: { oData: params } })) as AxiosResponse<
        ODataResponse<FAQ[]>
      >;
      return {
        items: data.value,
        count: data['@odata.count'],
      };
    },
    // getList
    async getFaqList({ rootGetters }, params) {
      const accessToken = rootGetters['auth/accessToken'];
      const { data } = (await faq.getFaqList(accessToken)({ params: { oData: params } })) as AxiosResponse<
        ODataResponse<FAQPage[]>
      >;
      return data
    },
    // getFAQAssign
    async getFAQAssign({ rootGetters }, FAQId): Promise<void> {
      const response = (await faq.getFAQAssign(FAQId)(rootGetters['auth/accessToken'])()) as AxiosResponse<FAQASSIGN>;
    },
    async getFAQAssignPages({ rootGetters }): Promise<FAQASSIGN> {
      const response = (await faq.getFAQAssignPages(rootGetters['auth/accessToken'])()) as AxiosResponse<FAQASSIGN>;
      return response.data
    },
// get
    async getFAQ({ rootGetters }, FAQId): Promise<FAQ> {
      const response = (await faq.getFAQContent(FAQId)(rootGetters['auth/accessToken'])()) as AxiosResponse<FAQ>;
      return response.data;
    },
    async deleteFAQ({ rootGetters }, FAQId) {
      return await faq.deleteFAQContent(FAQId)(rootGetters['auth/accessToken'])();
    },

    async updateFAQInstances({ rootGetters }, data: updateFAQInstanceData): Promise<void> {
      await faq.updateFAQInstances(data.faqId)(rootGetters['auth/accessToken'])(data.list);
    },
    async getFAQInstances({ rootGetters }, data: FAQPage) {
      const response = (await faq.getFAQInstances(rootGetters['auth/accessToken'])({data})) as AxiosResponse<Question[]>;
      return response.data;
    },
  },
};

export default module;
