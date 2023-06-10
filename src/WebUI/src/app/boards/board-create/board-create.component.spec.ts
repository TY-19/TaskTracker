import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardCreateComponent } from './board-create.component';

describe('BoardCreateComponent', () => {
  let component: BoardCreateComponent;
  let fixture: ComponentFixture<BoardCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BoardCreateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BoardCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
