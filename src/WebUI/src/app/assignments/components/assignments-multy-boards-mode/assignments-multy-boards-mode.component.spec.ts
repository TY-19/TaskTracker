import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentsMultyBoardsModeComponent } from './assignments-multy-boards-mode.component';

describe('AssignmentsMultyBoardsModeComponent', () => {
  let component: AssignmentsMultyBoardsModeComponent;
  let fixture: ComponentFixture<AssignmentsMultyBoardsModeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssignmentsMultyBoardsModeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentsMultyBoardsModeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
