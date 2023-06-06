import { Component, OnInit } from '@angular/core';
import { BoardService } from './board.service';
import { Board } from '../models/board';
import { CurrentUserService } from '../auth/currentuser.service';

@Component({
  selector: 'tt-boards',
  templateUrl: './boards.component.html',
  styleUrls: ['./boards.component.scss']
})
export class BoardsComponent implements OnInit {

  boards!: Board[];

  constructor(private boardService: BoardService,
    public currentUserService: CurrentUserService) { 

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

  isAdmin() : boolean {
    return this.currentUserService.isAdmin();
  }
  
  isManager() : boolean {
    return this.currentUserService.isManager();
  }

}
