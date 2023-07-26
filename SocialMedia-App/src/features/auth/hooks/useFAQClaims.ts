import { Ref } from '@vue/runtime-core';
import { useStore } from 'vuex';
import { PERMISSION_NAMES } from '@/constants/permissionNames';

interface FAQsClaimsHook {
  canEditAllFAQs: Ref<boolean>;
}

export default function (): FAQsClaimsHook {
  const store = useStore();
  return {
    canEditAllFAQs: store.getters['auth/authorizedUserCan'](PERMISSION_NAMES.FAQ_MANAGEMENT),
  };
}
