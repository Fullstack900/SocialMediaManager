import Question from './Question';

export default interface FAQ {
  // info
  id?: string;
  pageName: string;
  pageState: string;
  isEnabledSEO: boolean;
  seoUrl?: string;
  seoHeading?: string;
  seoDescription?: string;
  seoKeywords?: string;
  createdUserId?: string;
  createdUserName?: string;
  noOfQuestions: number;
  //Questions
  faqInstanceEntities?: Question[];
}
