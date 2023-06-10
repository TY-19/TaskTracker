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
    private auth: AuthService) { 
  }

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    this.boardService.getBoards()
      .subscribe({
        next: response => this.boards = response,
        error: error => console.log(error)
      });
  }

  deleteBoard(id: number) {
    if(confirm("Do you really want to delete the board? All tasks will be deleted as well"))
    {
      this.boardService.deleteBoard(id.toString()).subscribe({
        next: () => this.getData()
      });
    }
  }

  isAdmin() : boolean {
    return this.auth.isAdmin();
  }
  
  isManager() : boolean {
    return this.auth.isManager();
  }

}
