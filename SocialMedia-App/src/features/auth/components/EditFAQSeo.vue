<template>
  <BlockUI :blocked="blockContainer">
    <div class="grid flex-column curriculum-wrapper w-50">
      <div class="mb-3">
        <InputSwitch v-model="isEnabledSEOValue" name="isEnabledSEO" on-label="Yes" off-label="No" />
        <label class="switch-label">
          {{
            isEnabledSEOValue === true
              ? t('web.faq_pagebuilder_management.enable_seo')
              : t('web.faq_pagebuilder_management.disable_seo')
          }}
        </label>
      </div>

      <TextInput
        v-model="seoUrlValue"
        :counterhidden="true"
        :maxlength="50"
        :label="t('web.faq_pagebuilder_management.url.label')"
        :placeholder="t('web.faq_pagebuilder_management.url.placeholder')"
        :validations="URLValidations"
      />

      <TextInput
        v-model="seoTagValue"
        :counterhidden="true"
        :maxlength="50"
        :label="t('web.faq_pagebuilder_management.htmlTag.label')"
        :placeholder="t('web.faq_pagebuilder_management.htmlTag.placeholder')"
      />

      <TextAreaInput
        v-model="descriptionValue"
        :label="t('web.faq_pagebuilder_management.description.label')"
        :placeholder="t('web.faq_pagebuilder_management.description.placeholder')"
        :maxlength="1000"
        name="courseLearnAbout"
      />

      <TextAreaInput
        v-model="keywordsValue"
        :label="t('web.faq_pagebuilder_management.keywords.label')"
        :placeholder="t('web.faq_pagebuilder_management.keywords.placeholder')"
        :maxlength="1000"
        name="courseLearnAbout"
      />
    </div>
  </BlockUI>
</template>

<i18n global>
{
  "en": {
    "web": {
      "faq_pagebuilder_management": {
        "enable_seo": "Enable SEO", 
        "disable_seo": "Disable SEO", 
        "url": {
          "label": "SEO URL",
          "placeholder": "URL",
        },
        "htmlTag": {
          "label": "HTML-Tag H1",
          "placeholder": "HTML-Tag",
        },
        "description": {
          "label": "SEO Description",
          "placeholder": "Description",
        },
        "keywords": {
          "label": "SEO Keywords",
          "placeholder": "Keywords",
        },    
      },
    }
  },
  "ar": {
    "web": {
      "faq_pagebuilder_management": {
        "enable_seo": "تشغيل تحسين محركات البحث", 
        "disable_seo": "تعطيل  تحسين محركات البحث", 
        "url": {
          "label": "رابط   تحسين محركات البحث",
          "placeholder": "رابط",
        },
        "htmlTag": {
          "label": "HTML-Tag H1",
          "placeholder": "HTML-Tag",
        },
        "description": {
          "label": "وصف  تحسين محركات البحث",
          "placeholder": "الوصف",
        },
        "keywords": {
          "label": "الكلمات المفتاحية لتحسين محركات البحث",
          "placeholder": "الكلمات المفتاحية",
        },    
      },
    }
  }
}
</i18n>

<script lang="ts">
import { defineComponent, PropType, computed, ref } from 'vue';
import InputSwitch from 'primevue/inputswitch';
import TextInput from '@/components/molecules/TextInput.vue';
import TextAreaInput from '@/components/molecules/TextAreaInput.vue';
import { useI18n } from 'vue-i18n';
import FAQ from '../models/FAQ';
import BlockUI from 'primevue/blockui';
import { helpers } from '@vuelidate/validators';

export default defineComponent({
  name: 'EditFAQSEO',
  components: {
    TextInput,
    InputSwitch,
    TextAreaInput,
    BlockUI,
  },
  props: {
    modelValue: {
      type: Object as PropType<FAQ>,
      default: () => ({}),
    },
  },
  emits: ['update:modelValue'],

  setup(props, { emit }) {
    const { t } = useI18n();
    const blockContainer = ref(false);
    const seoUrlValue = computed({
      get: () => props.modelValue.seoUrl,
      set: (val) => emit('update:modelValue', { ...props.modelValue, seoUrl: val }),
    });

    const seoTagValue = computed({
      get: () => props.modelValue.seoHeading,
      set: (val) => emit('update:modelValue', { ...props.modelValue, seoHeading: val }),
    });

    const descriptionValue = computed({
      get: () => props.modelValue.seoDescription,
      set: (val) => emit('update:modelValue', { ...props.modelValue, seoDescription: val }),
    });

    const keywordsValue = computed({
      get: () => props.modelValue.seoKeywords,
      set: (val) => emit('update:modelValue', { ...props.modelValue, seoKeywords: val }),
    });

    const isEnabledSEOValue = computed({
      get: () => props.modelValue.isEnabledSEO,
      set: (val) => emit('update:modelValue', { ...props.modelValue, isEnabledSEO: val }),
    });

    isEnabledSEOValue.value = true;
    const urlRegex = ref<RegExp>(/^(?:https?:\/\/)?(?:www\.)?[a-zA-Z0-9-]+(?:\.[a-zA-Z]+)+[^\s]*$/);
    
    const URLValidations = computed(() => ({
      minValue: helpers.withMessage(
        t('web.organusation_contact.url_validation_message'),
        (val: string) => !val || urlRegex.value.test(val)
      ),
    }));
    return {
      t,
      isEnabledSEOValue,
      blockContainer,
      seoUrlValue,
      seoTagValue,
      descriptionValue,
      keywordsValue,
      URLValidations,
    };
  },
});
</script>

<style lang="scss" scoped>
.w-50 {
  width: 50% !important;
}

.switch-label {
  position: relative;
  top: -8px;
  right: -8px;
  font-size: 14px;
}
[dir='rtl'] .switch-label {
  right: unset;
  left: -8px;
}
</style>
