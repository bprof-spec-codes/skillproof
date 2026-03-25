import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionBankDetails } from './question-bank-details';

describe('QuestionBankDetails', () => {
  let component: QuestionBankDetails;
  let fixture: ComponentFixture<QuestionBankDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [QuestionBankDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionBankDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
