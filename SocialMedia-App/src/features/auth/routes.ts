import { RouteRecordRaw } from 'vue-router';
import { ifAuthorized } from '@/router/guards';
import { LAYOUTS } from '@/constants/layouts';
import { CATEGORIES } from '@/constants/categories';
import { FAQ_ROUTE_NAMES } from './routeNames';
import { PERMISSION_NAMES } from '@/constants/permissionNames';
import { FAQTabKeys } from '../faq/models/FAQTabs';
import { LEARNER_CATEGORIES } from '@/constants/categories';

const getTabParamString = () => `:tab(${Object.values(FAQTabKeys).join('|')})`;

const routes: Array<RouteRecordRaw> = [
  {
    path: '/faq',
    name: FAQ_ROUTE_NAMES.FAQ,
    component: () => import(/* webpackChunkName: "faq" */ './views/FAQ.vue'),
    meta: {
      layout: LAYOUTS.LEFT_SIDEBAR,
      pageTitleKey: 'web.faq.title',
      menuItemKey: 'web.menu.admin_faq.menu_title',
      menuCategory: CATEGORIES.GENERAL,
      withFooterActions: true,
    },
    beforeEnter: [ifAuthorized],
  },
  {
    path: `/faq/create/${getTabParamString()}`,
    name: FAQ_ROUTE_NAMES.FAQCREATE,
    component: () => import(/* webpackChunkName: "faq_create" */ './views/FAQUpsert.vue'),
    meta: {
      layout: LAYOUTS.LEFT_SIDEBAR,
      pageTitleKey: 'web.faq.create_title',
      inMenuItem: FAQ_ROUTE_NAMES.FAQ,
      backPath: FAQ_ROUTE_NAMES.FAQ,
      withFooterActions: true,
      byPermissions: [PERMISSION_NAMES.FAQ_MANAGEMENT],
    },
    props: true,
    beforeEnter: [ifAuthorized],
  },
  {
    path: `/faq/:id/edit/${getTabParamString()}`,
    name: FAQ_ROUTE_NAMES.FAQEDIT,
    component: () => import(/* webpackChunkName: "faq_edit" */ './views/FAQUpsert.vue'),
    meta: {
      layout: LAYOUTS.LEFT_SIDEBAR,
      pageTitleKey: 'web.faq.edit_title',
      inMenuItem: FAQ_ROUTE_NAMES.FAQ,
      backPath: FAQ_ROUTE_NAMES.FAQ,
      withFooterActions: true,
      byPermissions: [PERMISSION_NAMES.FAQ_MANAGEMENT],
    },
    props: true,
    beforeEnter: [ifAuthorized],
  },
  {
    path: `/faq/:id/view`,
    name: FAQ_ROUTE_NAMES.FAQVIEW,
    component: () => import(/* webpackChunkName: "faq_view" */ './views/FAQUpsert.vue'),
    meta: {
      layout: LAYOUTS.LEFT_SIDEBAR,
      pageTitleKey: 'web.faq.view_title',
      inMenuItem: FAQ_ROUTE_NAMES.FAQ,
      backPath: FAQ_ROUTE_NAMES.FAQ,
      byPermissions: [PERMISSION_NAMES.FAQ_MANAGEMENT],
    },
    props: true,
    beforeEnter: [ifAuthorized],
  },
];

export default [...routes];

export const publicRoutes = [
  {
    path: '/learner-faq',
    name: FAQ_ROUTE_NAMES.LEARNERFAQ,
    component: () => import(/* webpackChunkName: "liveSession" */ './views/LearnerFAQ.vue'),
    meta: {
      layout: LAYOUTS.LEFT_SIDEBAR,
      pageTitleKey: 'web.pricing_page.title',
      menuItemKey: 'web.menu.live_session.menu_title',
      menuItem: LEARNER_CATEGORIES.FAQ,
      byPermissions: [PERMISSION_NAMES.FAQ_MANAGEMENT],
    },
    beforeEnter: [ifAuthorized],
  },
];
