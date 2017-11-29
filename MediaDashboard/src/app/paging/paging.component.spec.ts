import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PagingComponent } from './paging.component';

describe('PagingComponent', () => {
  let component: PagingComponent<any>;
  let fixture: ComponentFixture<PagingComponent<any>>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PagingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PagingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
