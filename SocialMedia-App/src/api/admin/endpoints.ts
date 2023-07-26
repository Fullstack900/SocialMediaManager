import auth from '@/features/auth/endpoints/authEndpoints';
import employee from '@/features/configuration/users/endpoints/employeeEndpoints';
import organisation from '@/features/configuration/users/endpoints/organisationEndpoints';
import permissions from '@/features/configuration/users/endpoints/permissionsEndpoints';
import roles from '@/features/configuration/users/endpoints/rolesEndpoints';
import file from '@/features/file/endpoints/fileEndpoints';
import profile from '@/features/profile/endpoints/profileEndpoints';
import countries from '@/features/profile/endpoints/countriesEndpoints';
import learnerProfile from '@/features/profile/endpoints/learnerEndpoints';
import dictionaries from '@/features/configuration/dictionaries/endpoints/dictionariesEndpoints';
import courses from '@/features/contents/courses/endpoints/coursesEndpoints';
import chapter from '@/features/contents/courses/endpoints/chapterEndpoints';
import coupon from '@/features/contents/courses/endpoints/couponEndpoints';
import learner from '@/features/mylearnings/mycourses/endpoints/learnerEndpoint';
import landing from '@/features/configuration/webSite/endpoints/landingEndpoints';
import faq from '@/features/faq/endpoints/faqEndpoints';
import log from '@/features/logs/endpoints/logEndpoints';
import subscription from '@/features/billing/subscriptions/endpoints/subscriptionEndpoints';
import landingView from '@/features/landing/endpoints/landingEndpoints';
import grade from '@/features/configuration/dictionaries/grades/endpoints/gradeEndpoints';
import currency from '@/features/configuration/localisation/endpoints/currencyEndpoints';
import language from '@/features/configuration/localisation/endpoints/languageEndpoints';
import liveSession from '@/features/livesessions/endpoints/liveSessionEndpoints';
import support from '@/features/support/endpoints/supportEndpoints';
import userSupport from '@/features/support/endpoints/supportEndpoints';
import casecategory from '@/features/configuration/dictionaries/cases/endpoints/casesEndpoints';
import certificate from '@/features/contents/certificates/endpoints/certificates';
import systemMaintenance from '@/features/systemmaintenance/endpoints/systemMaintenanceEndpoints';
import notificationTemplate from '@/features/notificationTemplate/endpoints/NotificationTemplateEndpoints'
import settings from '@/features/configuration/localisation/endpoints/settingsEndpoints'

export {
  auth,
  permissions,
  roles,
  file,
  employee,
  organisation,
  profile,
  countries,
  dictionaries,
  courses,
  chapter,
  coupon,
  learnerProfile,
  learner,
  landing,
  faq,
  subscription,
  landingView,
  grade,
  log,
  currency,
  liveSession,
  language,
  support,
  casecategory,
  userSupport,
  certificate,
  systemMaintenance,
  notificationTemplate,
  settings,
}