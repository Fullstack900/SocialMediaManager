<template>
  <div class="align-center">
    <div class="medium-heading-w-48-size">
      <h3>{{ t('web.faq_page.page_title') }}</h3>
    </div>
    <div v-if="!isHideSubheadingValue" class="answer-here-light-text">
      <span class="p-muted">{{ t('web.faq_page.sub_title') }}</span>
      <a href="#">support@edyo.com</a>
    </div>
    <div class="accordion-main-box faq-main-box">
      <div class="faq-content-box accordion-content-box">
        <Accordion :active-index="0">
          <AccordionTab v-for="tab in pageContent" :key="tab.id" :header="tab.question">
            <p v-html="tab.answer"></p>
          </AccordionTab>
        </Accordion>
      </div>
    </div>

    <div class="faq-after-desc answer-here-light-text">
      <span>{{ t('web.faq_page.sub_title') }}</span>
      <a href="#">support@edyo.com</a>
    </div>
    <button class="p-button p-button-secondry px-5">{{ t('web.faq_page.help_btn') }}</button>
  </div>
</template>

<i18n global>
  {
    "en": {
      "web": {
        "faq_page": {
          "page_title": "Frequently Asked Questions",
          "sub_title": "In case you don't find an answer here, write to us at ",
          "help_btn": "Go to help center"
        }
      }
    },
    "ar": {
      "web": {
        "faq_page": {
          "page_title": "الأسئلة الشائعة",
          "sub_title": "في حال لم تجد إجابة هنا, اكتب لنا على",
          "help_btn": "اذهب إلى مركز ألدعم"
        }
      }
    }
  }
  </i18n>

<script lang="ts">
import { useI18n } from 'vue-i18n';
import { useStore } from 'vuex';
import { defineComponent, ref, onBeforeMount, onMounted, computed } from 'vue';
import useErrorHandling from '@/hooks/useErrorHandling';
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';
import Question from '../models/Question';
import { faq } from '@/api/admin/endpoints';

export default defineComponent({
  name: 'ContactPage',
  components: {
    Accordion,
    AccordionTab,
  },
  props: {
    pageName: {
      type: String,
      default: '',
    },
    isHideSubheading: {
      type: Boolean,
      default: false,
    },
    id: {
      type: String,
      default: '',
    },
  },
  setup(props) {
    const store = useStore();
    const { t } = useI18n();
    const { handleError } = useErrorHandling();
    const pageContent = ref<Question[]>();
    const isHideSubheadingValue = computed(() => {
      return props.isHideSubheading;
    });

    const fetchFaqPage = async () => {
      if (props.id) {
        try {
          let result = (await faq.getFAQInstances(store.getters['auth/accessToken'])({
            id: props.id,
            pageName: props.pageName,
          })) as any;
          pageContent.value = result.data;
        } catch (reason: any) {
          handleError(reason);
        }
      }
    };
    
    const wait = 1000;
    onMounted(async () => {
      setTimeout(async()=>{
        await fetchFaqPage();
      },wait)
    });

    return {
      t,
      pageContent,
      isHideSubheadingValue,
    };
  },
});
</script>

<style lang="scss">
.p-accordion-header-link {
  justify-content: space-between;
  // flex-direction: row-reverse;
  margin-bottom: 24px;

  .p-accordion-toggle-icon {
    font-weight: 600;
  }

  .p-accordion-header-text {
    font-weight: 800;
    font-size: 24px;
  }

  .pi-chevron-down:before {
    content: '\e903';
  }

  .pi-chevron-right:before {
    content: '\e902';
  }
}

.p-accordion-content {
  text-align: left;
}

.p-accordion-tab {
  padding-bottom: 20px;
  border-bottom: 1px solid #e9e6e6;
}

.align-center {
  text-align: center;
  align-content: center;
}

.course-title {
  font-family: 'Sofia Pro';
  font-style: normal;
  font-weight: 600;
  font-size: 36px;
  line-height: 56px;
}
</style>
