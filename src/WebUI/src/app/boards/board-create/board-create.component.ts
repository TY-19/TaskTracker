import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { BoardService } from '../board.service';
import { Router } from '@angular/router';
import { Board } from 'src/app/models/board';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-board-create',
  templateUrl: './board-create.component.html',
  styleUrls: ['./board-create.component.scss']
})
export class BoardCreateComponent implements OnInit {

  board!: Board;
  form!: UntypedFormGroup;

  constructor(private router: Router,
    private boardService: BoardService) { 

    }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void
  {
    this.form = new UntypedFormGroup({
      name: new UntypedFormControl("", [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        CustomValidators.boardNameValidator()
      ])
    });
  }

  onSubmit(): void {
    if(this.form.valid)
    {
      const board: Board = { id: 0,
        name: this.form.controls['name'].value };
      this.createBoard(board);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private createBoard(board: Board) {
    this.boardService.createBoard(board)
      .subscribe(() => { this.router.navigate(['/boards']) });
  }
}
