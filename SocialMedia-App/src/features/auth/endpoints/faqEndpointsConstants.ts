import config from '@/api-config.json';

const FAQ_BASE_URL = process.env.NODE_ENV === 'production' ? 'landing' : config.IsdevServerBaseUrl ? `${config.devServerBaseUrl}:7078` : 'https://localhost:7078';

export const FAQ_GET_LIST_API_URL = FAQ_BASE_URL + '/FAQs';
export const FAQ_GET_API_URL = FAQ_BASE_URL + '/FAQ';
export const FAQ_POST_API_URL = FAQ_BASE_URL + '/FAQ';
export const FAQ_PUT_API_URL = FAQ_BASE_URL + '/FAQ';
export const FAQINSTANCE_UPDATE_API_URL = FAQ_BASE_URL + '/FAQInstance';
export const FAQINSTANCE_NAMES_API_URL = FAQ_BASE_URL + '/FAQInstanceNames';
export const FAQ_LIST_GET_API = FAQ_BASE_URL + '/FAQList';
export const FAQ_GET_ASSIGN_API= FAQ_BASE_URL + '/AssignFAQ';
export const FAQ_GET_ASSIGNEDFAQPAGES_API = FAQ_BASE_URL + '/AssignedFAQPages';
