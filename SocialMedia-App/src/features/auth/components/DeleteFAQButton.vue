<template>
  <teleport v-if="loadTeleport" to="#internal-header-notification">
    <transition name="fade">
      <p v-if="error" class="notification p-error">{{ error }}</p>
    </transition>
  </teleport>
  <Button
    type="button"
    v-bind="$attrs"
    :disabled="disabled || loading"
    class="p-button-rounded p-button-text p-error employee-delete-button"
    :class="{
      'p-disabled': disabled || loading,   
    }"
    @click.stop.prevent="openConfirm"
  ><img src="@/assets/red-trash.png" alt="" /></Button>
</template>
<i18n global>
{
  "en": {
    "web": {
      "faqs": {
        "delete_confirmation_title": "Delete FAQ?",
        "delete_confirmation_message": "Please confirm deletion of the FAQ",
        "delete_confirmation_accept_btn": "Delete",
        "delete_confirmation_reject_btn": "No"
      }
    }
  },
  "ar": {
    "web": {
      "faqs": {
        "delete_confirmation_title": "حذف الأسئلة الشائعة؟",
        "delete_confirmation_message": "يرجى تأكيد حذف الأسئلة الشائعة",
        "delete_confirmation_accept_btn": "حذف",
        "delete_confirmation_reject_btn": "لا"
      }
    }
  }
}
</i18n>
<script lang="ts">
import { defineComponent, ref } from 'vue';
import Button from 'primevue/button';
import useErrorHandling from '@/hooks/useErrorHandling';
import { useConfirm } from 'primevue/useconfirm';
import { useStore } from 'vuex';
import { useI18n } from 'vue-i18n';
import TeleportHook from '@/hooks/useTeleportDelay';
export default defineComponent({
  name: 'DeleteFAQButton',
  components: {
    Button,
  },
  inheritAttrs: false,
  props: {
    faqId: {
      type: String,
      required: true,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['deleted'],
  setup(props, { emit }) {
    const { loadTeleport } = TeleportHook();
    const { t } = useI18n();
    const store = useStore();
    const confirm = useConfirm();
    const loading = ref(false);
    const { error, handleError } = useErrorHandling();

    const openConfirm = () => {
      loading.value = true;
      confirm.require({
        message: t('web.faqs.delete_confirmation_message'),
        header: t('web.faqs.delete_confirmation_title'),
        acceptLabel: t('web.faqs.delete_confirmation_accept_btn'),
        rejectLabel : t('web.faqs.delete_confirmation_reject_btn'),
        group: 'confirmation',
        accept: async () => {
          try {
            await store.dispatch('faq/deleteFAQ', props.faqId);
            emit('deleted');
          } catch (reason: any) {
            handleError(reason);
          } finally {
            loading.value = false;
            confirm.close();
          }
        },
        reject: () => {
          loading.value = false;
          confirm.close();
        },
      });
    };
    return {
      openConfirm,
      loading,
      loadTeleport,
      error,
    };
  },
});
</script>
<style lang="scss" scoped>

</style>
