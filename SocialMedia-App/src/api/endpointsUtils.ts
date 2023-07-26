import { UNREACHABLE_INDEX } from '@/constants/numbers';

export const checkUrl =
  (testString: string): ((url: string) => boolean) =>(url: string): boolean => (url && url.indexOf(testString)) > UNREACHABLE_INDEX;
