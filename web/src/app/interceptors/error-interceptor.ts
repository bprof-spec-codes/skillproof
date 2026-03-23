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
      let userMessage = "An unknown error occurred."

      if (err.status === 0) {
        userMessage = "Network error: the server is unreachable (CORS or offline)."
      }
      else if (err.error?.message || err.error?.error) {
        userMessage = err.error.message ?? err.error.error
      } else if (err.status === 401) {
        userMessage = "Session expired or invalid. Please log in again."
        localStorage.removeItem('skillProof_token')
        router.navigate(['/login'])
      } else if (err.status === 403) {
        userMessage = "You do not have permission to perform this action."
      } else if (err.status === 404) {
        userMessage = "The resource was not found."
      } else if (err.status >= 500) {
        userMessage = "A server error occurred. Please try again later."
      }
      
      modal.open({
          message: userMessage,
          autoClose: true,
          duration: 3000,
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
