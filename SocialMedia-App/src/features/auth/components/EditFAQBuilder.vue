<template>
  <BlockUI :blocked="blockContainer">
    <div class="grid flex-column curriculum-wrapper">
      <div class="col">
        <div class="mb-3">
          <DropDownInput
            v-model="pageState"
            name="pageState"
            :options="statuses"
            :label="t('web.faq_pagebuilder_management.status')"
            class="w-25"
          />
        </div>
        <TextInput
          v-model.trim.lazy="pageNameValue"
          class="w-25"
          name="pageName"
          :counterhidden="true"
          validation-scope="FAQBody"
          :maxlength="1000"
          :label="t('web.faq_pagebuilder_management.pageName.label')"
          :placeholder="t('web.faq_pagebuilder_management.pageName.placeholder')"
          :required="true"
        />
        <div class="col w-full chapters-list questions-list">
          <Draggable v-model="questionValue" :group="$attrs.group" :scroll-sensitivity="200" :bubble-scroll="true">
            <template #item="{ element, index }">
              <Panel
                :key="element.questionNo"
                :toggleable="!element.disableToggle"
                :collapsed="element.collapsed"
                class="section-item mb-4"
                :class="{
                  'p-panel-edit-mode': element.editMode,
                  'p-panel-header-expanded': !element.collapsed && !element.disableToggle,
                }"
              >
                <template #header>
                  <span class="handle">
                    <img src="@/assets/dotsAccordion.png" alt="" />
                  </span>
                  <span class="ps-l">Question {{ index + 1 }}</span>
                  <span class="ps-r">
                    <Button
                      type="button"
                      class="p-button-rounded p-button-text chapter-edit-button"
                      @click="removeQuestion(element.questionNo)"
                    >
                      <img src="@/assets/red-trash.png" alt="" />
                    </Button>
                    <label class="switch-label">
                      {{
                        element.isActive === true
                          ? t('web.faq_pagebuilder_management.enable_faq')
                          : t('web.faq_pagebuilder_management.disable_faq')
                      }}
                    </label>
                    <InputSwitch v-model="element.isActive" on-label="Yes" off-label="No" />
                  </span>
                </template>

                <div class="pl-5">
                  <TextInput
                    v-model.lazy="element.question"
                    :counterhidden="true"
                    :maxlength="2000"
                    validation-scope="FAQBody"
                    :label="t('web.faq_pagebuilder_management.question.label')"
                    :placeholder="t('web.faq_pagebuilder_management.question.placeholder')"
                    :required="true"
                  />
                  <label>{{ t('web.faq_pagebuilder_management.answer.label') }}</label>
                  <Editor
                    v-model="element.answer"
                    :placeholder="t('web.faq_pagebuilder_management.answer.placeholder')"
                    :maxlength="5000"
                    validation-scope="FAQBody"
                    :required="true"
                  >
                    <template #toolbar>
                      <span class="ql-formats">
                        <button v-tooltip.bottom="'Bold'" class="ql-bold"></button>
                        <button v-tooltip.bottom="'Italic'" class="ql-italic"></button>
                        <button v-tooltip.bottom="'List'" class="ql-list" value="ordered"></button>
                        <button v-tooltip.bottom="'List'" class="ql-list" value="bullet"></button>
                        <button v-tooltip.bottom="'Link'" class="ql-link"></button>
                      </span>
                    </template>
                  </Editor>
                </div>
              </Panel>
            </template>
          </Draggable>
        </div>
        <Button
          type="button"
          class="mr-2 create-chapter-button add-question-btn"
          :label="t('web.faq_pagebuilder_management.new_question')"
          @click="addQuestion()"
        />
      </div>
    </div>
  </BlockUI>
</template>

<i18n global>
{
  "en": {
    "web": {
      "faq_pagebuilder_management": {
        "new_question": "Add Questions",
        "enable_faq": "On", 
        "disable_faq": "Off", 
        "status": "Status",
        "page_state":{
          "published": "Published",
          "draft": "Draft",
          "error": "Error",
          "pending": "Pending",
        },
        "pageName": {
          "label": "F.A.Q Page Name",
          "placeholder": "Page name",
        },
        "question": {
          "label": "Question",
          "placeholder": "Question",
        },
        "answer": {
          "label": "Answer",
          "placeholder": "Text",
        },     
      },
    }
  },
  "ar": {
    "web": {
      "faq_pagebuilder_management": {
        "new_question": "أضف سؤال",
        "pageName": {
          "label": "اسم الصفحة الأسئلة الشائعة",
          "placeholder": "عنوان الصفحة",
        },
        "question": {
          "label": "سؤال",
          "placeholder": "سؤال",
        },
        "answer": {
          "label": "إجابة",
          "placeholder": "نص",
        },     
      },
    }
  }
}
</i18n>

<script lang="ts">
import { defineComponent, PropType, computed, ref } from 'vue';
import TextInput from '@/components/molecules/TextInput.vue';
import Editor from 'primevue/editor';
import { useI18n } from 'vue-i18n';
import FAQ from '../models/FAQ';
import BlockUI from 'primevue/blockui';
import Draggable from 'vuedraggable';
import Panel from 'primevue/panel';
import InputSwitch from 'primevue/inputswitch';
import Question from '../models/Question';
import Button from 'primevue/button';
import { ONE, UNREACHABLE_INDEX } from '@/constants/numbers';
import useErrorHandling from '@/hooks/useErrorHandling';
import DropDownInput from '@/components/molecules/DropDownInput.vue';
// import { PageState } from '@/features/configuration/webSite/pages/models/enums/pageState';

export default defineComponent({
  name: 'EditFAQBuilder',
  components: {
    TextInput,
    Editor,
    BlockUI,
    Draggable,
    Panel,
    Button,
    InputSwitch,
    DropDownInput,
  },
  inheritAttrs: false,
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
    const { handleError } = useErrorHandling();
    const pageNameValue = computed({
      get: () => props.modelValue.pageName,
      set: (val) => emit('update:modelValue', { ...props.modelValue, pageName: val }),
    });

    const questionValue = computed({
      get: () => props.modelValue.faqInstanceEntities,
      set: (val) => emit('update:modelValue', { ...props.modelValue, faqInstanceEntities: val }),
    });

    const addQuestion = () => {
      let count = props.modelValue.faqInstanceEntities ? props.modelValue.faqInstanceEntities.length : ONE;
      let question = ref<Question>({
        id: undefined,
        questionNo: count + ONE,
        question: '',
        answer: '',
        isActive: true,
        collapsed: false,
      });
      questionValue?.value?.push(question.value);
    };

    const removeQuestion = (questionNo: number) => {
      if (props.modelValue.faqInstanceEntities && props.modelValue.faqInstanceEntities.length === ONE) {
        handleError('FAQ Question must not be empty.');
        return;
      }
      let question =
        questionValue?.value?.findIndex((x) => x.questionNo == questionNo) === undefined
          ? UNREACHABLE_INDEX
          : questionValue?.value?.findIndex((x) => x.questionNo == questionNo);
      questionValue?.value?.splice(question, ONE);
    };

    const pageState = computed({
      get: () => props.modelValue.pageState,
      set: (val) => emit('update:modelValue', { ...props.modelValue, pageState: val }),
    });

    const statuses = ['Draft', 'Published'];

    return {
      t,
      addQuestion,
      removeQuestion,
      blockContainer,
      questionValue,
      pageNameValue,
      pageState,
      statuses,
      props,
    };
  },
});
</script>

<style lang="scss" scoped>
.w-25 {
  width: 25% !important;
}

.handle {
  letter-spacing: -0.5rem;
}

.create-chapter-button {
  width: fit-content;
}
.add-question-btn {
  padding: 11px 16px;
}
.questions-list {
  padding-left: 0;
}

:deep(.p-panel .p-panel-header) {
  background: none !important;
  border-radius: 8px;
  padding: 1rem 1rem;
  border-bottom: none;
}

:deep(.section-item) {
  border-bottom: 1px solid #e1e6ea;
  border-radius: 8px;
}

:deep(.p-panel-content) {
  border-bottom: none;
}

.ps-l {
  position: absolute;
  left: 50px;
}

.ps-r {
  position: absolute;
  right: 50px;
  display: flex;
  align-items: center;
  gap: 6px;
}
.switch-status-label {
  position: relative;
  top: -8px;
  right: -8px;
  font-size: 14px;
}

.publish-status-published,
.publish-status-error,
.publish-status-pending,
.publish-status-draft {
  justify-content: center;
  text-align: start;
  padding: 0.125rem 1rem 0.375rem 1rem;
  margin: 0rem 0.25rem 0rem 0.25rem;
  border-width: 1px;
  border-radius: 1rem;
  border-style: solid;
  font-size: 0.75rem;
  font-weight: 600;
}

.publish-status-published {
  border-color: var(--el-brand-color4);
  color: var(--el-brand-color4);
}

.publish-status-pending,
.publish-status-draft {
  border-color: $edyo-blue;
  color: $edyo-blue;
}

.publish-status-error {
  border-color: $edyo-red;
  color: $edyo-red;
}
</style>
