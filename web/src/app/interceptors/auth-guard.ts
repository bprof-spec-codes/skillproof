import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service';

export const authGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService)
  const router = inject(Router)

  if (!auth.isLoggedIn()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } })
    return false
  }

  const required = route.data?.['roles'] as string[] | string | undefined
  if (required && !auth.hasRole(required)) {
    router.navigate(['/login'], { queryParams: { reason: 'forbidden' } })
    return false
  }
  return true
};
