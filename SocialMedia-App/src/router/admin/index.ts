
import { adminRoutes as SignInRoutes } from '@/features/auth/routes';
import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import SecurityRoutes from '@/features/security/routes';
import ConfigurationRoutes from '@/features/configuration/routes';
import FaqRoutes from '@/features/faq/routes';
import ContentsRoutes from '@/features/contents/routes';
import BillingRoutes from '@/features/billing/routes';
import { RoutesAdmin } from '@/features/profile/routes';
import LandingRoutes from '@/features/landing/routes';
import systemMaintenanceRoutes from '@/features/systemmaintenance/routes';
import LiveSession from '@/features/livesessions/routes';
import SystemLogs from '@/features/logs/routes';
import AdminTimeTableRoutes from '@/features/timetable/admin/routes';
import NotificationTemplateRoutes from '@/features/notificationTemplate/routes';
import ReportRoutes from '@/features/reports&analytics/routes';
import { SupportAdminRoutes } from '@/features/support/routes';


const routes: Array<RouteRecordRaw> = [
  ...SignInRoutes,
  ...RoutesAdmin,
  ...SecurityRoutes,
  ...ConfigurationRoutes,
  ...FaqRoutes,
  ...ContentsRoutes,
  ...BillingRoutes,
  ...LandingRoutes,
  ...systemMaintenanceRoutes,
  ...NotificationTemplateRoutes,
  ...SystemLogs,
  ...LiveSession,
  ...AdminTimeTableRoutes,
  ...ReportRoutes,
  ...SupportAdminRoutes,
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes: routes,
});

export default router;
