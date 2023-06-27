import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserBoardsAddComponent } from './user-boards-add.component';

describe('UserBoardsAddComponent', () => {
  let component: UserBoardsAddComponent;
  let fixture: ComponentFixture<UserBoardsAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserBoardsAddComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserBoardsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
