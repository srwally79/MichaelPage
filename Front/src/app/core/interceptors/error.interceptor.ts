import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ApiErrorResponse } from '../models/api.models';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError = error.error as Partial<ApiErrorResponse> | null;
      const isSessionCheck = request.url.endsWith('/api/auth/session');
      if (isSessionCheck && (error.status === 401 || error.status === 404)) {
        authService.markUnauthenticated();
      } else if (error.status === 401) {
        authService.markUnauthenticated();
        notificationService.showError('Tu sesión expiró. Inicia sesión nuevamente.');
        void router.navigateByUrl('/login');
      } else if (error.status === 400) {
        notificationService.showError(apiError?.message ?? 'Revisa los datos ingresados.');
      } else if (error.status === 0) {
        notificationService.showError('No se pudo conectar con el servidor.');
      } else {
        notificationService.showError(apiError?.message ?? 'Ocurrió un error al procesar la solicitud.');
      }
      return throwError(() => error);
    })
  );
};
