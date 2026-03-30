import { TestBed } from '@angular/core/testing';

import { QuestionBankService } from './question-bank-service';

describe('QuestionBankService', () => {
  let service: QuestionBankService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QuestionBankService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
