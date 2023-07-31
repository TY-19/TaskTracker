import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardDisplayOptionsComponent } from './board-display-options.component';

describe('BoardDisplayOptionsComponent', () => {
  let component: BoardDisplayOptionsComponent;
  let fixture: ComponentFixture<BoardDisplayOptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BoardDisplayOptionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BoardDisplayOptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
