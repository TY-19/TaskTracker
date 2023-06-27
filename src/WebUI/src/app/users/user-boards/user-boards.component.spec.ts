import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserBoardsComponent } from './user-boards.component';

describe('UserBoardsComponent', () => {
  let component: UserBoardsComponent;
  let fixture: ComponentFixture<UserBoardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserBoardsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserBoardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
