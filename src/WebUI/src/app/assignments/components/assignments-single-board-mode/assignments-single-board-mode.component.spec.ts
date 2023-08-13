import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentsSingleBoardModeComponent } from './assignments-single-board-mode.component';

describe('AssignmentsSingleBoardModeComponent', () => {
  let component: AssignmentsSingleBoardModeComponent;
  let fixture: ComponentFixture<AssignmentsSingleBoardModeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssignmentsSingleBoardModeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentsSingleBoardModeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
