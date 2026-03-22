import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment.development';
import { catchError, throwError } from 'rxjs';
import { ModalService } from '../services/modal-service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router)
  const modal = inject(ModalService)
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      let userMessage = "Ismeretlen hiba történt."

      if (err.status === 0) {
        userMessage = "Hálózati hiba: a szerver nem elérhető (CORS vagy offline)."
      }
      else if (err.error?.message || err.error?.error) {
        userMessage = err.error.message ?? err.error.error
      } else if (err.status === 401) {
        userMessage = "A munkamenet lejárt vagy érvénytelen. Jelentkezz be újra."
        localStorage.removeItem('skillProof_token')
        router.navigate(['/login'])
      } else if (err.status === 403) {
        userMessage = "Nincs jogosultságod a művelethez."
      } else if (err.status === 404) {
        userMessage = "Az erőforrás nem található."
      } else if (err.status >= 500) {
        userMessage = "Szerverhiba történt. Próbáld meg később."
      }
      
      modal.open({
          message: userMessage,
          autoClose: true,
          duration: 2000,
          size: 'md',
          type: 'error'
        })

      console.error("[HTTP ERROR]", {
        method: req.method,
        url: req.urlWithParams,
        status: err.status,
        error: err
      });

      

      (err as any).userMessage = userMessage
      return throwError(() => err) 
    }),
  )
};
