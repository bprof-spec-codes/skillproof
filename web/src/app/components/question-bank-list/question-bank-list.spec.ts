import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionBankList } from './question-bank-list';

describe('QuestionBankList', () => {
  let component: QuestionBankList;
  let fixture: ComponentFixture<QuestionBankList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [QuestionBankList],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionBankList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
