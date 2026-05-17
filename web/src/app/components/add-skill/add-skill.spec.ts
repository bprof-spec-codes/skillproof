import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSkill } from './add-skill';

describe('AddSkill', () => {
  let component: AddSkill;
  let fixture: ComponentFixture<AddSkill>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddSkill],
    }).compileComponents();

    fixture = TestBed.createComponent(AddSkill);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
