import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentEditComponent } from './assignment-edit.component';

describe('AssignmentEditComponent', () => {
  let component: AssignmentEditComponent;
  let fixture: ComponentFixture<AssignmentEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssignmentEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
