import { Component, OnInit } from '@angular/core';
import { BoardService } from './board.service';
import { Board } from '../models/board';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'tt-boards',
  templateUrl: './boards.component.html',
  styleUrls: ['./boards.component.scss']
})
export class BoardsComponent implements OnInit {

  boards!: Board[];

  constructor(private boardService: BoardService,
    private authService: AuthService) {
  }

  ngOnInit(): void {
    this.getData();
  }

  private getData(): void {
      this.boardService.getBoardsOfTheEmployee()
        .subscribe(response => this.boards = response);
  }

  deleteBoard(id: number): void {
    if(confirm("Do you really want to delete the board? All tasks will be deleted as well"))
    {
      this.boardService.deleteBoard(id)
        .subscribe(() => this.getData());
    }
  }
  get isAdminOrManager(): boolean {
    return this.authService.isAdmin() || this.authService.isManager();
  }
}
