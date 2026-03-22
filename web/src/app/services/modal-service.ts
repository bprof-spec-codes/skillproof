import { Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';

export interface ModalConfig {
  message: string;
  autoClose?: boolean;
  duration?: number; // ms
  size?: 'sm' | 'md' | 'lg';
  type?: 'success' | 'error' | 'warning' | 'info';
} // lehet még bővíteni

@Injectable({
  providedIn: 'root',
})
export class ModalService {

  private modalState = new BehaviorSubject<ModalConfig | null>(null)
  modalState$ = this.modalState.asObservable()

  private timeoutId: any;

  open(config: ModalConfig) {
    // reset előző timer
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }

    this.modalState.next(null);

    setTimeout(() => {
      this.modalState.next({ ...config });
    });

    if (config.autoClose) {
      this.timeoutId = setTimeout(() => {
        this.close();
      }, config.duration || 3000);
    }
  }

  close() {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
    this.modalState.next(null);
  }
}
