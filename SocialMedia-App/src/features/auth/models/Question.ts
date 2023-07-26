export default interface Question {
  id?: string;
  // info
  questionNo?: number;
  question?: string;
  answer?: string;
  isActive: boolean;
  collapsed?: boolean;
}
