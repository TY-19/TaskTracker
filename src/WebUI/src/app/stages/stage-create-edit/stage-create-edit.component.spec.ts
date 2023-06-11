import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StageCreateEditComponent } from './stage-create-edit.component';

describe('StageCreateEditComponent', () => {
  let component: StageCreateEditComponent;
  let fixture: ComponentFixture<StageCreateEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StageCreateEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StageCreateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
