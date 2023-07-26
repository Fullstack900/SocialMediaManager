<template>
  <div
    class="p-4 courses-page-wrapper h-full"
    :class="{ 'flex align-items-center justify-content-center': !FAQs.length }"
  >
    <teleport v-if="loadTeleport && canEditAllFAQs" :to="headerActionTeleportTarget">
      <router-link :to="createFAQRoute" class="p-button course-create-link">
        {{ t('web.faq.create_button') }}
      </router-link>
    </teleport>
    <teleport v-if="loadTeleport" to="#internal-header-additional">
      <div class="flex justify-content-start w-full internal-header-inner-div internal-header-resp">
        <SearchInput
          v-model="filters['global'].value"
          :placeholder="t('web.course_management.search_placeholder')"
          label=""
          class="w-full md:w-auto"
          @input="onFilterChange"
        />
        <div v-if="hasAppliedFilters" class="spacer mx-2"></div>
        <AppliedFilters
          v-model="filters"
          :initial-filters="initialFilters"
          class="mb-4"
          @filterRemoved="onFilterChange"
        />
      </div>
    </teleport>
    <EmptyState
      v-if="!FAQs.length"
      :title="t('web.faq.faqs_list.empty_state')"
      :subtitle="t('web.faq.faqs_list.empty_state_description')"
      :default-action-label="t('web.faq.create_button')"
      :action-handler="goToCreateFAQ"
      :no-action="!canEditAllFAQs"
    />
    <DataTable
      v-else
      v-model:filters="filters"
      :value="FAQs"
      :total-records="totalItems"
      :paginator="totalItems > 5"
      lazy
      :rows="5"
      :paginator-template="responsivePaginationConfig"
      :rows-per-page-options="[5, 10, 20, 50]"
      filter-display="menu"
      :global-filter-fields="['Login']"
      :responsive-layout="responsiveTableLayout"
      class="employees-table"
      removable-sort
      :breakpoint="`${scssBreakpoints.sm}px`"
      @page="onPageChange"
      @sort="onSort"
      @filter="onFilterChange"
    >
      <template #loading>{{ t('web.configuration_employees.employee_list.loading_state') }}</template>
      <Column
        v-for="column in columns"
        :key="column.key"
        :field="column.key"
        :header="column.header"
        :filter-field="column.key"
        :sortable="column.sortable"
        :data-type="column.dataType"
        :header-class="`${column.key}-course-column`"
        :body-class="`${column.key}-course-column`"
      >
        <template v-if="column.key !== 'actions'" #filter="{ filterModel }">
          <TextInput
            v-if="column.key === 'pageName'"
            v-model="filterModel.value"
            class="p-column-filter mb-0"
            :placeholder="t('web.filters.placeholders.pageName')"
            label=""
            type="text"
          />
          <Dropdown
            v-else-if="column.key === 'pageState'"
            v-model="filterModel.value"
            :options="allStatuses"
            option-label="name"
            :option-value="'value'"
            :placeholder="t('web.filters.placeholders.statuses')"
          ></Dropdown>
          <TextInput
            v-else-if="column.key === 'noOfQuestions'"
            v-model="filterModel.value"
            class="p-column-filter mb-0"
            :placeholder="t('web.filters.placeholders.noOfQuestions')"
            type="number"
          />
        </template>
        <!-- Action Column  -->
        <template #body="slotProps">
          <template v-if="column.key === 'actions'">
            <template v-if="canEditAllFAQs">
              <router-link
                :to="editRoute(slotProps.data['id'])"
                :class="
                  'p-button p-button-text course-edit-link ' +
                  slotProps.data['pageName'].replaceAll(' ', '').toLowerCase()
                "
                :data-employee-id="slotProps.data['id']"
              >
                <!-- <i class="pi pi-pencil"></i> -->
                <img src="@/assets/red-edit.png" alt="" />
              </router-link>
              <DeleteFAQButton
                :class="'course-delete-button ' + slotProps.data['pageName'].replaceAll(' ', '').toLowerCase()"
                :faq-id="slotProps.data['id']"
                @deleted="onDeleteFAQ"
              />
            </template>
            <router-link
              v-else
              :to="viewRoute(slotProps.data['id'])"
              :class="
                'p-button p-button-text course-view-link ' +
                slotProps.data['pageName'].replaceAll(' ', '').toLowerCase()
              "
              :data-role-id="slotProps.data['id']"
            >
              <i class="pi pi-eye"></i>
            </router-link>
          </template>
          <template v-else-if="column.key === 'pageState'">
            <span class="inline-flex align-items-center">
              <span>
                {{ t(`web.faq.faqs_list.${slotProps.data[column.key].toLowerCase()}_pageState`) }}
              </span>
            </span>
          </template>
          <template v-else-if="column.key === 'pageName'">
            <div class="flex align-items-center">
              <div class="spacer mx-1"></div>
              <span>{{ slotProps.data[column.key] }}</span>
            </div>
          </template>
          <template v-else-if="column.key === 'noOfQuestions'">
            <div class="flex align-items-center">
              <div class="spacer mx-1"></div>
              <span>{{ slotProps.data[column.key] }}</span>
            </div>
          </template>
        </template>
      </Column>
    </DataTable>
  </div>
</template>
<i18n global>
  {
    "en": {
      "web": {
        "faq": {
            "title": "F.A.Q",
            "create_button": "Create",
            "faqs_list": {
                    "empty_state": "F.A.Q. list is empty",
                    "empty_state_description": "You don`t have any F.A.Qs. Please, create your first F.A.Q.",
                    "name_header": "F.A.Q Name",
                    "pageState_header": "Status",
                    "question_header": "Questions",
                    "actions_header": "Action",
                    "draft_pageState": "Draft",
                    "published_pageState": "Published",
                    "loading_state": "Loading FAQ data. Please wait.",
                  },
        },
        "course_management": {
          "auto_saved": "",
          "search_placeholder": "Search",
          
        },
        "filters": {
          "labels": {
            "pageName": "Name",
            "pageState": "PageState",
            "noOfQuestions": "Questions",
          },
          "placeholders": {
            "pageName": "Search by name",
            "noOfQuestions": "Search by number",
            "statuses": "Select a status",
          }
        }
      }
    }
  }
  </i18n>
<script lang="ts">
import { computed, defineComponent, onBeforeMount, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { FAQ_ROUTE_NAMES } from '../routeNames';
import FAQ from '../models/FAQ';
import useTeleportDelay from '@/hooks/useTeleportDelay';
import EmptyState from '@/components/organisms/EmptyState.vue';
import { useRouter } from 'vue-router';
import useHeaderActionsTarget from '@/hooks/useHeaderActionsTeleportTarget';
import usePermissions from '@/features/auth/hooks/usePermissions';
import useFAQsClaims from '@/features/faq/hooks/useFAQClaims';
import { isEmpty } from 'lodash';
import useErrorHandling from '@/hooks/useErrorHandling';
import store from '@store/index';
import useOrderBy from '@/hooks/useOrderBy';
import useFilters, { GLOBAL_SEARCH_FILTER_KEY } from '@/hooks/useODataFilter';
import { FilterMatchMode, FilterOperator } from 'primevue/api';
import usePagination from '@/hooks/usePagination';
import useCssBreakpoints from '@/hooks/useCssBreakpoints';
import DataTable from 'primevue/datatable';
import { DataTableColumn } from '@/models/DataTableColumn';
import Column from 'primevue/column';
import SearchInput from '@/components/molecules/SearchInput.vue';
import TextInput from '@/components/molecules/TextInput.vue';
import Dropdown from 'primevue/dropdown';
import DeleteFAQButton from '../components/DeleteFAQButton.vue';
import { FAQTabKeys } from '../models/FAQTabs';
import AppliedFilters from '@/components/molecules/AppliedFilters.vue';

const POSSIBLE_STATUSES = {
  DRAFT: 'Draft',
  PUBLISH: 'Published',
};

export default defineComponent({
  name: 'ManageCourses',
  components: {
    DataTable,
    Column,
    TextInput,
    SearchInput,
    Dropdown,
    EmptyState,
    DeleteFAQButton,
    AppliedFilters,
  },
  setup() {
    const { t } = useI18n();
    const router = useRouter();
    const { handleError } = useErrorHandling();

    // permissions
    usePermissions();
    const canEditAllFAQs = useFAQsClaims();
    const userId = store.getters['auth/currentUserId']();

    // courses fetching
    const FAQs = ref<FAQ[]>([]);
    const courseRequestParams = computed(() => {
      const params = { ...paginationQuery.value, ...orderByQuery.value };
      if (!isEmpty(mappedFilters.value)) {
        params.filter = mappedFilters.value;
      }
      return params;
    });
    const fetchFAQs = async () => {
      try {
        const { items, count } = await store.dispatch('faq/getFAQs', courseRequestParams.value);
        FAQs.value = items;
        totalItems.value = count;
      } catch (reason: any) {
        handleError(reason);
      }
    };
    onBeforeMount(async () => {
      fetchFAQs();
    });

    // pagination
    const { page, totalItems, isPaginatorVisible, paginationQuery, onPageChange } = usePagination(fetchFAQs);
    const responsivePaginationConfig = computed(() =>
      smAndLower.value
        ? 'PrevPageLink PageLinks NextPageLink'
        : 'PrevPageLink PageLinks NextPageLink RowsPerPageDropdown'
    );

    // ordering
    const { orderByQuery, onSort } = useOrderBy(fetchFAQs, {});

    // filters
    const initialFilters = {
      [GLOBAL_SEARCH_FILTER_KEY]: { value: null, matchMode: FilterMatchMode.CONTAINS,type: 'string' },
      pageName: {
        operator: FilterOperator.AND,
        constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
        type: 'string',
      },
      noOfQuestions: {
        operator: FilterOperator.AND,
        excludeFromGlobalFilter: true,
        constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS, type: Number }],
      },
      pageState: {
        operator: FilterOperator.AND,
        excludeFromGlobalFilter: true,
        constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS, fieldName: 'pageState' }],
        type: 'enum',
      },
    };
    const { hasAppliedFilters, filters, mappedFilters, onFilterChange } = useFilters(initialFilters, fetchFAQs);

    // table columns
    const columns = ref<DataTableColumn[]>([
      {
        key: 'pageName',
        header: t('web.faq.faqs_list.name_header'),
        sortable: true,
        dataType: 'text',
      },
      {
        key: 'noOfQuestions',
        header: t('web.faq.faqs_list.question_header'),
        sortable: true,
        dataType: 'numeric',
      },
      {
        key: 'pageState',
        header: t('web.faq.faqs_list.pageState_header'),
        sortable: true,
        dataType: 'enum',
      },
      { key: 'actions', header: t('web.faq.faqs_list.actions_header') },
    ]);
    const { smAndLower, scssBreakpoints } = useCssBreakpoints();
    const responsiveTableLayout = computed(() => (smAndLower.value ? 'stack' : 'scroll'));

    const createFAQRoute = {
      name: FAQ_ROUTE_NAMES.FAQCREATE,
      params: { tab: FAQTabKeys.pageBuilder },
    };

    const editRoute = (FAQId: string) => {
      return { name: FAQ_ROUTE_NAMES.FAQEDIT, params: { id: FAQId, tab: FAQTabKeys.pageBuilder } };
    };

    const viewRoute = (FAQId: string) => {
      return { name: FAQ_ROUTE_NAMES.FAQVIEW, params: { id: FAQId, tab: FAQTabKeys.pageBuilder } };
    };
    const goToCreateFAQ = () => {
      router.push(createFAQRoute);
    };

    // teleport init
    const { loadTeleport } = useTeleportDelay();
    const { headerActionTeleportTarget } = useHeaderActionsTarget();

    const onDeleteFAQ = () => {
      fetchFAQs();
    };

    return {
      loadTeleport,
      FAQs,
      t,
      editRoute,
      viewRoute,
      goToCreateFAQ,
      canEditAllFAQs,
      createFAQRoute,
      headerActionTeleportTarget,
      columns,
      filters,
      initialFilters,
      totalItems,
      onPageChange,
      onFilterChange,
      allStatuses: [
        {
          name: t('web.faq.faqs_list.draft_pageState'),
          value: POSSIBLE_STATUSES.DRAFT,
        },
        {
          name: t('web.faq.faqs_list.published_pageState'),
          value: POSSIBLE_STATUSES.PUBLISH,
        },
      ],
      responsiveTableLayout,
      onSort,
      onDeleteFAQ,
      scssBreakpoints,
      hasAppliedFilters,
      responsivePaginationConfig,
    };
  },
});
</script>

<style lang="scss">
.courses-page-wrapper {
  .p-panel.courses-card {
    .p-panel-header {
      border: none;
      border-radius: 0;
      padding-left: 0;
      padding-right: 0;
      background: transparent;
    }

    .p-panel-content {
      border: none;
      border-radius: 0;
      padding-left: 0;
      padding-right: 0;
      background: transparent;
    }

    .course-card-content {
      border: 1px solid var(--el-borders-bg);
    }
  }
}
</style>
