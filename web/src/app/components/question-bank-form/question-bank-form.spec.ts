import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionBankForm } from './question-bank-form';

describe('QuestionBankForm', () => {
  let component: QuestionBankForm;
  let fixture: ComponentFixture<QuestionBankForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [QuestionBankForm],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionBankForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
