<template>
  <teleport v-if="loadTeleport" to="#internal-header-notification">
    <transition name="fade">
      <p v-if="saved" class="notification p-success">{{ t('web.faq.auto_saved') }}</p>
    </transition>
  </teleport>
  <teleport v-if="loadTeleport" to="#internal-header-additional">
    <TabView
      class="tabs-header-wrapper course-details-tabs"
      :active-index="activeTabIndex.value"
      @tab-change="changeTab"
    >
      <TabPanel v-for="tab in tabs" :key="tab.key + 'header'" :header="tab.title" :class="tab.class"></TabPanel>
    </TabView>
  </teleport>
  <teleport v-if="loadTeleport && !viewMode" :to="headerActionTeleportTarget">
    <div class="align-items-center flex">
      <DeleteFAQButton v-if="model.id" :faq-id="model.id" class="mx-2" @deleted="onBack" />
      <Button v-if="!model.id" type="button" class="p-button-text mx-2" :label="cancelLabel" @click="onBack()" />
      <Button
        v-if="!model.id"
        type="button"
        class="mx-2 create-course-button"
        :label="createLabel"
        @click="onSave(true)"
      />
      <Button v-else type="button" class="mx-2 save-course-button" :label="saveLabel" @click="onBack()" />
    </div>
  </teleport>
  <div class="grid mx-4 content-header course-edit-page-wrapper">
    <div class="tab-title">
      {{ tabs[activeIndex].title }}
    </div>
    <ExternalTabSwitcher v-model="activeIndex" :tabs="tabs" class="tabs-switcher" @on-change="changeTab" />
  </div>
  <TabView :active-index="activeIndex" class="p-tabview--no-nav faq-questions-list-tab">
    <TabPanel v-for="tab in tabs" :key="tab.key">
      <div :class="tab.componentClass">
        <component :is="tab.component" v-model="model" :view-mode="viewMode" />
      </div>
    </TabPanel>
  </TabView>
</template>
<i18n global>
{
  "en": {
    "web": {
      "faq": {
        "create_title": "Create F.A.Q",
        "faq_pagebuilder_tab_title" : "PageBuilder",
        "faq_seo_tab_title" : "SEO",
        "auto_saved": "\u2713 Auto saved",        
        "edit_title": "Edit F.A.Q",
        "create": "Create",
        "save": "Save",
        "cancel": "Cancel",
        "name": {
          "label": "F.A.Q Page Name",
          "placeholder": "Page Name",
        },
      }
    }
  },
  "ar": {
    "web": {
      "faq": {
        "create_title": "إنشاء الأسئلة الشائعة",
        "faq_pagebuilder_tab_title" : "منشئ الصفحة",
        "faq_seo_tab_title" : "تحسين محرك البحث",
        "auto_saved": "\u2713 حفظ تلقائي",        
        "edit_title": "تعديل الأسئلة الشائعة",
        "create": "إنشاء",
        "save": "حفظ",
        "cancel": "إلغاء",
        "name": {
          "label": "اسم الصفحة الأسئلة الشائعة",
          "placeholder": "اسم الصفحة",
        },
      }
    }
  }
}
</i18n>
<script lang="ts">
import { computed, defineComponent, onBeforeMount, PropType, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import TabView from 'primevue/tabview';
import TabPanel from 'primevue/tabpanel';
import { FAQ_ROUTE_NAMES } from '@/features/faq/routeNames';
import { useRouter, useRoute } from 'vue-router';
import FAQ from '../models/FAQ';
import { FAQTabKeys } from '@/features/faq/models/FAQTabs';
import { cloneDeep, debounce, isEmpty, startCase } from '@/utils/lodash';
import useTabs from '@/hooks/useTabs';
import { FIVE_SECONDS, ONE_SECOND } from '@/constants/time';
import Button from 'primevue/button';
import ExternalTabSwitcher from '@/components/molecules/ExternalTabSwitcher.vue';
import { useVuelidate } from '@vuelidate/core';
import useHeaderActionsTarget from '@/hooks/useHeaderActionsTeleportTarget';
import DeleteFAQButton from '../components/DeleteFAQButton.vue';
import EditFAQBuilder from '../components/EditFAQBuilder.vue';
import EditFAQSeo from '../components/EditFAQSeo.vue';
import useErrorHandling from '@/hooks/useErrorHandling';
import useTeleportDelay from '@/hooks/useTeleportDelay';
import { TabViewEvent } from '@/models/TabViewEvent';
import { faq } from '@endpoints/endpoints';
import store from '@store/index';
import { difference } from '@/utils/objectDiff';
import usePreloader from '@/hooks/usePreloader';
import useToasts from '@/hooks/useToasts';

export default defineComponent({
  name: 'FAQUpsert',
  components: {
    TabView,
    TabPanel,
    Button,
    ExternalTabSwitcher,
    DeleteFAQButton,
    EditFAQBuilder,
    EditFAQSeo,
  },
  props: {
    id: {
      type: String,
      default: '',
    },
    tab: {
      type: String as PropType<FAQTabKeys>,
      default: FAQTabKeys.pageBuilder,
    },
  },
  setup(props) {
    const { t } = useI18n();
    const { loadTeleport } = useTeleportDelay();

    //separate property needed as a workaround for proper returning to invalid tab
    const activeTabIndex = computed({
      get: () => activeIndex,
      set: (val) => {
        activeIndex.value = val.value;
      },
    });

    const tabs = computed(() => [
      {
        key: FAQTabKeys.pageBuilder,
        title: t('web.faq.faq_pagebuilder_tab_title'),
        class: 'course-curriculum-tab-link',
        componentClass: 'm-0 p-4 curriculum-tab',
        component: `EditFAQ${startCase(FAQTabKeys.pageBuilder)}`,
      },
      {
        key: FAQTabKeys.seo,
        title: t('web.faq.faq_seo_tab_title'),
        class: 'course-curriculum-tab-link',
        componentClass: 'm-0 p-4 seo-tab',
        component: `EditFAQ${startCase(FAQTabKeys.seo)}`,
      },
    ]);

    const { activeIndex, setActive } = useTabs(tabs.value.findIndex((el) => el.key === props.tab));

    const saved = ref(false);

    const changeTab = async (event: TabViewEvent) => {
      v$.value.$touch();
      if (!modelValid.value) {
        event.index = 0;
      } else if (!model.value.id) {
        if (!model.value.faqInstanceEntities?.length) {
          handleError('FAQ Question must not be empty.');
          return false;
        }
        onSave();
      }
      router.push({
        ...route,
        params: {
          ...route.params,
          tab: tabs.value[event.index].key,
        },
      });
      setActive(event);
    };

    const createMode = computed(() => {
      return !props.id;
    });

    const v$ = useVuelidate({ $lazy: !createMode.value, $scope: 'FAQBody' });

    const model = ref<FAQ>({
      id: undefined,
      pageName: '',
      pageState: 'Draft',
      noOfQuestions: 0,
      isEnabledSEO: false,
      seoUrl: '',
      seoHeading: '',
      seoDescription: '',
      seoKeywords: '',
      //Questions
      faqInstanceEntities: [],
    });

    const modelValid = computed(() => {
      return !v$.value.$invalid && !loading.value;
    });

    const router = useRouter();
    const route = useRoute();
    const loading = ref(false);

    const onSave = async (exitAfterSave = false) => {
      v$.value.$touch();
      if (!modelValid.value) {
        return false;
      }
      if (!model.value.faqInstanceEntities?.length) {
        handleError('FAQ Question must not be empty.');
        return false;
      }
      loading.value = true;
      try {
        let result = (await faq.createFAQ(store.getters['auth/accessToken'])(model.value)) as any;
        model.value.id = result.data.id;
        oldVal = cloneDeep(model.value);
        if (exitAfterSave) onBack();
      } catch (reason: any) {
        setActive({ index: 0 } as any);
        handleError(reason.response.data.value);
      }
      loading.value = false;
    };

    const { headerActionTeleportTarget } = useHeaderActionsTarget();

    const viewMode = computed(() => {
      return route.name == FAQ_ROUTE_NAMES.FAQVIEW;
    });

    const onBack = () => {
      showPreloader();
      if (modelDirty.value) {
        hidePreloader();
      } else {
        hidePreloader();
        router.push({ name: router.currentRoute.value.meta.backPath as string });
      }
    };

    const createLabel = t('web.faq.create');
    const saveLabel = t('web.faq.save');
    const cancelLabel = t('web.faq.cancel');
    const { showPreloader, hidePreloader } = usePreloader();

    useToasts();

    const { $externalResults, error, handleError } = useErrorHandling();
    // provide('$externalResults', $externalResults);
    // //TODO investigate how we can utilize v$.value.$anyDirty for monitoring changes
    // // now Vuelidate only track items that marked as required
    const modelDirty = ref(false);

    let oldVal: FAQ = cloneDeep(model.value);

    onBeforeMount(async () => {
      if (props && props.id) {
        showPreloader();
        store
          .dispatch('faq/getFAQ', props.id)
          .then(function (faq) {
            model.value = faq;
            oldVal = cloneDeep(model.value);
          })
          .catch(function (reason: any) {
            handleError(reason);
          })
          .finally(() => {
            hidePreloader();
          });
      }
    });

    const setSaved = () => {
      saved.value = true;
      setTimeout(() => {
        saved.value = false;
      }, FIVE_SECONDS);
    };

    watch(
      model,
      () => {
        if (!loading.value && oldVal.id) {
          error.value = '';
          loading.value = true;
          Promise.all([saveFAQ()]).finally(() => {
            modelDirty.value = true;
            loading.value = false;
          });
        }
      },
      { deep: true }
    );

    const saveFAQ = debounce(async () => {
      v$.value.$touch();
      if (!modelValid.value) {
        return false;
      }
      const updatedFields = difference(model.value, oldVal) as unknown as FAQ;
      if (!isEmpty(updatedFields) && modelValid.value) {
        try {
          await store.dispatch('faq/updateFAQ', {
            faqId: model.value.id,
            delta: updatedFields,
          });
          oldVal = cloneDeep(model.value);
          setSaved();
        } catch (reason: any) {
          handleError(reason.response.data.value);
          oldVal = cloneDeep(model.value);
        }
      }
      modelDirty.value = false;
    }, ONE_SECOND);

    return {
      activeIndex,
      model,
      viewMode,
      tabs,
      activeTabIndex,
      loadTeleport,
      saved,
      showPreloader,
      saveLabel,
      createLabel,
      cancelLabel,
      t,
      setActive,
      changeTab,
      createMode,
      modelValid,
      // instanceModelValid,
      onSave,
      onBack,
      // onSaveQuestions,
      v$,
      headerActionTeleportTarget,
    };
  },
});
</script>

<style lang="scss">
.info-tab,
.details-tab {
  max-width: 548px;
}

.content-header {
  .tab-title {
    font-size: 1.125rem;
    font-weight: 400;
  }

  .tabs-switcher {
    cursor: pointer;
    margin-left: auto;
  }
}

.notification {
  font-size: 0.75rem;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

[dir='rtl'] .content-header {
  .tabs-switcher {
    margin-left: unset;
    margin-right: auto;
  }
}
@media (max-width: 991px) {
  .p-tabview-panels {
    .curriculum-tab {
      .curriculum-wrapper {
        .field {
          width: 50% !important;
        }
      }
    }
    .curriculum-wrapper {
      width: 100% !important;
    }
  }
}
@media (max-width: 767px) {
  .p-tabview-panels {
    .curriculum-tab {
      .curriculum-wrapper {
        .field {
          width: 100% !important;
        }
      }
    }
  }
}
</style>
