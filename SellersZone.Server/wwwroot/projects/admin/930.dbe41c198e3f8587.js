"use strict";(self.webpackChunkFastkart_Admin_SSR=self.webpackChunkFastkart_Admin_SSR||[]).push([[930],{5930:(y,f,i)=>{i.r(f),i.d(f,{CommissionModule:()=>E});var p=i(177),e=i(3487),u=i(1583),m=i(1635);let r=(()=>{class a{static{this.type="[Commission] Get"}constructor(s){this.payload=s}}return a})();var F=i(8141),t=i(4438),l=i(7871),g=i(1626);let c=(()=>{class a{constructor(s){this.http=s}getCommissionHistory(s){return this.http.get(`${l.c.URL}/commission.json`,{params:s})}static{this.\u0275fac=function(o){return new(o||a)(t.KVO(g.Qq))}}static{this.\u0275prov=t.jDH({token:a,factory:a.\u0275fac,providedIn:"root"})}}return a})(),h=class v{constructor(n){this.commissionService=n}static commission(n){return n.commission}getCommission(n,s){return this.commissionService.getCommissionHistory(s.payload).pipe((0,F.M)({next:o=>{n.patchState({commission:{data:o.data,total:o?.total?o?.total:o.data?.length}})},error:o=>{throw new Error(o?.error?.message)}}))}static{this.\u0275fac=function(s){return new(s||v)(t.KVO(c))}}static{this.\u0275prov=t.jDH({token:v,factory:v.\u0275fac})}};(0,m.Cg)([(0,e.rc)(r)],h.prototype,"getCommission",null),(0,m.Cg)([(0,e.MD)()],h,"commission",null),h=(0,m.Cg)([(0,e.Uw)({name:"commission",defaults:{commission:{data:[],total:0}}})],h);var M=i(67),O=i(628);class C{constructor(n,s){this.store=n,this.router=s,this.tableConfig={columns:[{title:"No.",dataField:"no",type:"no"},{title:"order_id",dataField:"order_id"},{title:"store_name",dataField:"store_name"},{title:"admin_commission",dataField:"admin_commission",type:"price"},{title:"vendor_commission",dataField:"vendor_commission",type:"price"},{title:"created_at",dataField:"created_at",type:"date"}],data:[],total:0}}ngOnInit(){this.commission$.subscribe(n=>{let s=n?.data?.filter(o=>(o.store_name=`<span class="text-capitalize">${o.store.store_name}</span>`,o.order_id=`<span class="fw-bolder">#${o.order.order_number}</span>`,o));this.tableConfig.data=n?s:[],this.tableConfig.total=n?n?.total:0})}onTableChange(n){this.store.dispatch(new r(n))}static{this.\u0275fac=function(s){return new(s||C)(t.rXU(e.il),t.rXU(u.Ix))}}static{this.\u0275cmp=t.VBU({type:C,selectors:[["app-commission"]],decls:2,vars:5,consts:[[3,"gridClass","title"],[3,"tableChanged","tableConfig","hasCheckbox","dateRange"]],template:function(s,o){1&s&&(t.j41(0,"app-page-wrapper",0)(1,"app-table",1),t.bIt("tableChanged",function(R){return o.onTableChange(R)}),t.k0s()()),2&s&&(t.Y8G("gridClass","col-sm-12")("title","commission_history"),t.R7$(),t.Y8G("tableConfig",o.tableConfig)("hasCheckbox",!1)("dateRange",!0))},dependencies:[M.O,O.k]})}}(0,m.Cg)([(0,e.l6)(h.commission)],C.prototype,"commission$",void 0);var S=i(8745);const A=[{path:"",component:C,canActivate:[S.L],data:{permission:"commission_history.index"}}];let D=(()=>{class a{static{this.\u0275fac=function(o){return new(o||a)}}static{this.\u0275mod=t.$C({type:a})}static{this.\u0275inj=t.G2t({imports:[u.iI.forChild(A),u.iI]})}}return a})();var I=i(1592);let E=(()=>{class a{static{this.\u0275fac=function(o){return new(o||a)}}static{this.\u0275mod=t.$C({type:a})}static{this.\u0275inj=t.G2t({imports:[p.MD,D,I.G,e.rK.forFeature([h])]})}}return a})()},8745:(y,f,i)=>{i.d(f,{L:()=>m});var p=i(4438),e=i(3487),u=i(1583);let m=(()=>{class r{constructor(t,l){this.store=t,this.router=l}canActivate(t,l){const g=this.store.selectSnapshot(d=>d.account).permissions?.map(d=>d?.name),c=t.data?.permission;return!!(!c||!Array.isArray(c)&&g?.includes(c)||Array.isArray(c)&&c?.length&&c.every(d=>g?.includes(d)))||(this.router.navigate(["/error/403"]),!1)}static{this.\u0275fac=function(l){return new(l||r)(p.KVO(e.il),p.KVO(u.Ix))}}static{this.\u0275prov=p.jDH({token:r,factory:r.\u0275fac,providedIn:"root"})}}return r})()}}]);