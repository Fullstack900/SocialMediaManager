<template>
  <div class="faq-main-small-content-box">
    <FAQPage
      :id="id"
      :page-name="FAQPageName"
      :is-hide-subheading="true"
    ></FAQPage>
  </div>

</template>

<i18n global>
  {
    "en": {
      "web": {
        "pricing_page": {
            "title": "FAQ",
            "discount_line": "Per month, paid once a year",
            "save_btn":"Save 25%",
            "start_btn":"Get Started",
            "demo_request":"Request Demo",
            "pay_type": {
              "pay_monthly": "Pay Monthly",
              "pay_annually": "Pay Annually",
            },
            "compare_btn": "See Our Compelete Feature Comparison",
            "empty_state": "Found nothing",
        }
      }
    },
    "ar": {
      "web": {
        "pricing_page": {
            "discount_line": "شهريًا ، تدفع مرة واحدة سنويًا",
            "save_btn":"وفر 25٪",
            "start_btn":"ابدأ ",
            "demo_request":"طلب عرض تجريبي",
            "pay_type": {
              "pay_monthly": "الدفع شهريا",
              "pay_annually": "الدفع سنويا",
            },
            "compare_btn": "عرض مقارنة كاملة بين الخواص والمميزات",
            "empty_state": "لم يتم العثور على شيء",
        }
      }
    }
  }
  </i18n>

<script lang="ts">
import { useI18n } from 'vue-i18n';
import { useStore } from 'vuex';
import { ref, onBeforeMount } from 'vue';
import useErrorHandling from '@/hooks/useErrorHandling';
import Button from 'primevue/button';
import InputSwitch from 'primevue/inputswitch';
import Card from '@/features/landing/components/PriceCard.vue';
//   import PricePlan from '../Models/PricePlan';
import PricePlan from '@/features/landing/Models/PricePlan';
import LANDING_ROUTE_NAMES from '@/features/landing/routeNames';
import FAQPage from '@/features/faq/views/FAQPage.vue';
import EmptyState from '@/components/organisms/EmptyState.vue';
import { PricingPageContent } from '@/features/configuration/webSite/pages/models/pricing/PricingPageContent';
import { PricingIndividualSection } from '@/features/configuration/webSite/pages/models/pricing/IndividualPlanModel';
import { ZERO } from '@/constants/numbers';
import { ItemModel } from '@/features/landing/Models/ItemModel';
import FAQAssign from '../models/FAQAssign';
import { PlanModel } from '@/features/configuration/webSite/pages/models/pricing/PlanModel';
import RadioButton from 'primevue/radiobutton';
import useCssBreakpoints from '@/hooks/useCssBreakpoints';

export default {
  components: {
    FAQPage,
  },
  setup() {
    const store = useStore();
    const { t } = useI18n();
    const { handleError } = useErrorHandling();
    const ActiveMobilePlan = ref('');
    const RadioInputs = ref<string[]>([]);
    const { lgAndLower } = useCssBreakpoints();

    const FAQSection = ref<FAQAssign>();
      const FAQPageName = ref<string>();
      const id = ref<string>();
    const fetchFaq = async () => {
      try {
        FAQSection.value = await store.dispatch('faq/getFAQAssignPages');
        console.log();
        id.value= FAQSection.value?.faqEntityId;
        FAQPageName.value = FAQSection.value?.faqSource;
      } catch (reason: unknown) {
        handleError(reason);
      }
    };
    onBeforeMount(async () => {
      await fetchFaq();
    });



    return {
      t,
      FAQPageName,
      id,
      PlanModel,
      RadioInputs,
      ActiveMobilePlan,
      lgAndLower,
    };
  },
};
</script>

<style lang="scss">
.pricing-sub-title-disc {
  max-width: 560px;
  margin: 0 auto;
  font-size: 20px;
}
.price-page-title {
  font-weight: 600;
  font-size: 2.5rem;
  text-align: center;
}

.sub-title {
  font-weight: 600;
  font-size: 1.5rem;
}

.p-button-bl {
  background: none;
  font-family: 'Sofia Pro';
  font-weight: 600;
  font-size: 14px;
  text-align: center;
  color: rgba(48, 74, 97, 0.5);
  border: none;
  padding: 2px 16px;
  border-radius: 8px;
  margin: 3px;
  line-height: 1.5rem;
  height: auto;
}

.p-button-bl.active {
  background: #d2721f;
  color: white;
  border: none;
}

.p-button-bl:hover {
  border: none;
  box-shadow: none;
}

@media screen and (min-width: 768px) {
  .md\:price-page-title-md {
    font-size: 3.5rem !important;
  }
}
</style>
