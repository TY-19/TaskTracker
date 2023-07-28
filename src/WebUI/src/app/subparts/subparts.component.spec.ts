import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubpartsComponent } from './subparts.component';

describe('SubpartsComponent', () => {
  let component: SubpartsComponent;
  let fixture: ComponentFixture<SubpartsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubpartsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SubpartsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
