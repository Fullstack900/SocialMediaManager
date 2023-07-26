import store from '@/store/index';
import { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import { AUTH_ROUTE_NAMES } from '@/features/auth/routeNames';
type AuthorizedGuard = (to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) => void;

export const ifAuthorized: AuthorizedGuard = ( 
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) => {
  if (store.getters['auth/isAuthorized']) {
    return next();
  }
  store.dispatch('auth/signOut');
  next({ name: AUTH_ROUTE_NAMES.SIGN_IN, params: { redirectTo: to.fullPath } });
};

type PermissionGuard = (to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) => void;
export const ifHasPermission: PermissionGuard = (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) => {
  const canViewRoute: boolean = ((to.meta.byPermissions as Array<string>) || []).some((permission) =>
    store.getters['auth/authorizedUserCan'](permission)
  );
  return to.meta.byPermissions && !canViewRoute ? next({ name: 'Home' }) : next();
};
