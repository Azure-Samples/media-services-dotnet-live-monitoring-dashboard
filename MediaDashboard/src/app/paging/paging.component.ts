import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-paging',
  templateUrl: './paging.component.html',
  styleUrls: ['./paging.component.css']
})
export class PagingComponent<T> implements OnInit {

  constructor() { }

  @Input()
  data: T[];

  @Input()
  pageSize: number;

  currentPage: number;

  pageData: T[]

  ngOnInit() {
  }

}
