import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentsAllBoardsComponent } from './assignments-all-boards.component';

describe('EmployeesAssignmentsComponent', () => {
  let component: AssignmentsAllBoardsComponent;
  let fixture: ComponentFixture<AssignmentsAllBoardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssignmentsAllBoardsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentsAllBoardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
