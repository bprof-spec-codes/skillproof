import { Component } from '@angular/core';
import { ModalConfig, ModalService } from '../../services/modal-service';
import { delay } from 'rxjs';

@Component({
  selector: 'app-modal',
  standalone: false,
  templateUrl: './modal.html',
  styleUrl: './modal.scss',
})
export class Modal {

   config: ModalConfig | null = null;

  constructor(public modalService: ModalService) {}

  close() {
    this.modalService.close();
  }

}
