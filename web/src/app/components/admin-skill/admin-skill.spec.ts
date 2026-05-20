import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminSkill } from './admin-skill';

describe('AdminSkill', () => {
  let component: AdminSkill;
  let fixture: ComponentFixture<AdminSkill>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminSkill],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminSkill);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
