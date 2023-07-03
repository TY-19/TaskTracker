import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignmentViewFullComponent } from './assignment-view.component';

describe('AssignmentViewFullComponent', () => {
  let component: AssignmentViewFullComponent;
  let fixture: ComponentFixture<AssignmentViewFullComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AssignmentViewFullComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignmentViewFullComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
