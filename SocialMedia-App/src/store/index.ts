import { createStore } from 'vuex';
import auth from '@/features/auth/store/authModule';
import role from '@/features/configuration/users/store/roleModule';
import permissions from '@/features/configuration/users/store/permissionModule';
import employees from '@/features/configuration/users/store/employeesModule';
import profile from '@/features/profile/store/profileModule';
import countries from '@/features/profile/store/countriesModule';
import dictionaries from '@/features/configuration/dictionaries/store/dictionariesModule';
import courses from '@/features/contents/courses/store/coursesModule';
import chapter from '@/features/contents/courses/store/chapterModule';
import coupon from '@/features/contents/courses/store/couponModule';
import landing from '@/features/configuration/webSite/store/landingModule';
import faq from '@/features/faq/store/faqModule';
import subscription from '@/features/billing/subscriptions/store/subscriptionModule';
import landingView from '@/features/landing/store/landingModule';
import grade from '@/features/configuration/dictionaries/grades/store/gradeModule';
import log from '@/features/logs/store/LogsModule';
import currency from '@/features/configuration/localisation/store/currencyModule';
import liveSession from '@/features/livesessions/store/liveSessionModule';
import language from '@/features/configuration/localisation/store/languageModule';
import support from '@/features/support/store/supportModule';
import timetable from '@/features/timetable/admin/store/timeTableModule';
import systemMaintenance from '@/features/systemmaintenance/store/systemMaintenanceModule';
import certificate from '@/features/contents/certificates/store/certificateModule';
import notificationTemplate from '@/features/notificationTemplate/store/notificationTemplateModule';
import organisation from '@/features/configuration/users/store/organisationModule';
import setting from '@/features/configuration/localisation/store/settingsModule';

const store = createStore({
  modules: {
    auth: {
      namespaced: true,
      ...auth,
    },
    role: {
      namespaced: true,
      ...role,
    },
    permissions: {
      namespaced: true,
      ...permissions,
    },
    employees: {
      namespaced: true,
      ...employees,
    },
    profile: {
      namespaced: true,
      ...profile,
    },
    countries: {
      namespaced: true,
      ...countries,
    },
    dictionaries: {
      namespaced: true,
      ...dictionaries,
    },
    courses: {
      namespaced: true,
      ...courses,
    },
    chapter: {
      namespaced: true,
      ...chapter,
    },
    coupon: {
      namespaced: true,
      ...coupon,
    },
    landing: {
      namespaced: true,
      ...landing,
    },
    faq: {
      namespaced: true,
      ...faq,
    },
    log: {
      namespaced: true,
      ...log,
    },
    subscription: {
      namespaced: true,
      ...subscription,
    },
    landingView: {
      namespaced: true,
      ...landingView,
    },
    grade: {
      namespaced: true,
      ...grade,
    },
    currency: {
      namespaced: true,
      ...currency,
    },
    liveSession: {
      namespaced: true,
      ...liveSession,
    },
    language: {
      namespaced: true,
      ...language,
    },
    support: {
      namespaced: true,
      ...support,
    },
    timetable: {
      namespaced: true,
      ...timetable,
    },
    certificate: {
      namespaced: true,
      ...certificate,
    },
    systemMaintenance: {
      namespaced: true,
      ...systemMaintenance,
    },
    notificationTemplate: {
      namespaced: true,
      ...notificationTemplate,
    },
    organisation: {
      namespaced: true,
      ...organisation,
    },
    setting: {
      namespaced: true,
      ...setting,
    }
  },
});
export default store;
