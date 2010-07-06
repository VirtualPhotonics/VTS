//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Cuccia.ArrayMath;
//using Vts.IO;
//using Vts.IO.Extensions;

//namespace Vts.MonteCarlo
//{
//    public partial class MonteCarloSimulation
//    {
//        public const double MU_LB = 0.01;

//        //struct vox_list
//        //{  /* used only by allvox */
//        //    public int ix, iy, iz;
//        //    public int col;
//        //    public int sa_idx;
//        //}
//        //LinkedList<vox_list> voxelLinkedList;

//        ///***********************************************************/
//        //void init_banana_allvox(out bvolume bananaptr, ref Output outptr)
//        //{
//        //    int num_sides = 6;
//        //    bananaptr = new bvolume();
//        //    /* use cylindrical data for cartesian data */
//        //    bananaptr.nx = 2 * tissptr.nr + 1;  /* center source */
//        //    bananaptr.ny = 1;
//        //    bananaptr.nz = tissptr.nz;
//        //    Console.WriteLine("banana:nx,ny,nz={0},{1},{2}", bananaptr.nx, bananaptr.ny, bananaptr.nz);
//        //    outptr.out_side_allvox = new double[num_sides][, ,];
//        //    outptr.in_side_allvox = new double[num_sides][, ,];
//        //    for (int i = 0; i < num_sides; i++)
//        //    {
//        //        outptr.out_side_allvox[i] = new double[bananaptr.nx, bananaptr.ny, bananaptr.nz];
//        //        outptr.in_side_allvox[i] = new double[bananaptr.nx, bananaptr.ny, bananaptr.nz];
//        //    }
//        //}

//        /**************************************************************/
//        void Compute_Prob_allvox()
//        {
//            int i, j, ktrk, jfix, num, curr_layer;
//            bool dead, debug, in_layer, next_same_vox, exit;
//            dead = debug = in_layer = next_same_vox = exit = false;
//            long N;
//            int ix, iy, iz;
//            ix = iy = iz = 0;
//            int nx, ny, nz, side = -99;
//            double this_x, this_y, this_z, next_x, next_y, next_z;
//            double w = 1.0, dx, dy, dz, xmid, ymid, zmid, xmid2, ymid2, zmid2;
//            double mins, tracklen;
//            double[] s = new double[6];
//            double phot_disc_wt;
//            double mu = 1;
//            int bdry_col = 0;
//            //struct vox_list *this_vox=NULL,*head;
//            double MIN_X, MAX_X, MIN_Y, MAX_Y, MIN_Z, MAX_Z;
//            MIN_X = -tissptr.nr * tissptr.dr - tissptr.dr / 2;
//            MAX_X = tissptr.nr * tissptr.dr + tissptr.dr / 2;
//            MIN_Y = -tissptr.nr * tissptr.dr - tissptr.dr / 2;
//            MAX_Y = tissptr.nr * tissptr.dr + tissptr.dr / 2;
//            MIN_Z = 0.0;
//            MAX_Z = tissptr.nz * tissptr.dz;
//            nx = bananaptr.nx;
//            ny = bananaptr.ny;
//            nz = bananaptr.nz;
//            dx = (MAX_X - MIN_X) / nx;
//            dy = (MAX_Y - MIN_Y) / ny;
//            dz = (MAX_Z - MIN_Z) / nz;
//            N = tissptr.num_photons;
//            num =histptr.num_pts_stored;
//            ++bananaptr.banana_photons;
//            phot_disc_wt = 1.0;
//            //in_layer=false;
//            //exit=f;  /* this needs to be 0 so that for/adj work simultaneously */
//            for (ktrk = 0; ktrk < num - 1; ++ktrk)
//            {  /* for all tracks */
//                if (!dead)
//                {
//                    this_x =histptr.xh[ktrk];
//                    this_y =histptr.yh[ktrk];
//                    this_z =histptr.zh[ktrk];
//                    next_x =histptr.xh[ktrk + 1];
//                    next_y =histptr.yh[ktrk + 1];
//                    next_z =histptr.zh[ktrk + 1];
//                    tracklen = Math.Sqrt((next_x - this_x) * (next_x - this_x) +
//                                  (next_y - this_y) * (next_y - this_y) +
//                                  (next_z - this_z) * (next_z - this_z));
//                    xmid = this_x;
//                    ymid = this_y;
//                    zmid = this_z;
//                    next_same_vox = false;
//                    /* DEBUG */
//                    /* if ((bananaptr.banana_photons>=1041)&&(ktrk>=270)) debug=1; 
//                    else debug=0;     */
//                    if (debug)
//                    {
//                        Console.WriteLine("N {0} TRACK {1}: this=({2},{3},{4})", bananaptr.banana_photons, ktrk, this_x, this_y, this_z);
//                        Console.WriteLine("                 : next=({0},{1},{2})", next_x, next_y, next_z);
//                        Console.WriteLine("phot_dist_wt={0}\n", phot_disc_wt);
//                    }
//                    /* flag if out sides */
//                    if ((Math.Abs(next_x) > MAX_X) || (Math.Abs(next_y) > MAX_Y))
//                    {
//                        in_layer = false;
//                        if (debug) Console.WriteLine("outside of layer ");
//                    }  /* if out sides */
//                    else
//                    { /* not out sides */
//                       histptr.boundary_col[ktrk] = 0;
//                        for (i = 1; i <= tissptr.num_layers; i++)
//                        {
//                            /* following based on "this" s.t. check later for weights agrees */
//                            if (Math.Abs(this_z - Tissue.Regions[i].zbegin) < 1e-9)
//                            {
//                               histptr.boundary_col[ktrk] = 1;
//                                if (debug) Console.WriteLine("boundary collision!");
//                            }
//                        }

//                        if (!in_layer)
//                        { /* check if enter layer */
//                            if ((MIN_Z == 0.0) && (ktrk == 0))
//                            { /* layer at origin */
//                                /* NOTE: this may not handle if passes thru 1st voxel */
//                                in_layer = true;
//                                ix = (int)Math.Floor((this_x - MIN_X) / dx);
//                                iy = (int)Math.Floor((this_y - MIN_Y) / dy);
//                                iz = 0;
//                                side = 0;
//                                /* adjoint */
//                                outptr.in_side_allvox[side][ix, iy, iz] += phot_disc_wt;
//                            }
//                            else
//                            { /* layer not at origin */
//                                s[0] = 0;
//                                if ((this_z <= MIN_Z) && (next_z > MIN_Z))
//                                    s[0] = (MIN_Z - this_z) / (next_z - this_z);
//                                else if ((this_z > MAX_Z) && (next_z < MAX_Z))
//                                    s[0] = (MAX_Z - this_z) / (next_z - this_z);
//                                /* determine point of intersection */
//                                if ((s[0] > 0) && (s[0] <= 1))
//                                {  /* crossed into layer */
//                                    xmid = this_x + s[0] * (next_x - this_x);
//                                    ymid = this_y + s[0] * (next_y - this_y);
//                                    zmid = this_z + s[0] * (next_z - this_z);
//                                    /* NOTE: ix,iy,iz always min of voxel */
//                                    ix = (int)Math.Floor((xmid - MIN_X) / dx);
//                                    iy = (int)Math.Floor((ymid - MIN_Y) / dy);
//                                    if (next_z > this_z)
//                                    { /* enter from top */
//                                        iz = 0;
//                                        side = 0;
//                                        /* adjoint */
//                                        if (in_layer)
//                                            outptr.in_side_allvox[side][ix, iy, iz] += phot_disc_wt;
//                                    }
//                                    else
//                                    { /* enter from bottom */
//                                        iz = nz - 1;
//                                        side = 5;
//                                        /* adjoint */
//                                        if (in_layer)
//                                            outptr.in_side_allvox[side][ix, iy, iz] += phot_disc_wt;
//                                    }
//                                    in_layer = true;
//                                }  /* crossed into layer */
//                            } /* layer not at origin */
//                            if ((debug) && (in_layer))
//                            {
//                                Console.WriteLine("enter layer at (x,y,z)=({0},{1},{2})", xmid, ymid, zmid);
//                                Console.WriteLine("enter layer at (ix,iy,iz)=({0},{1},{2})", ix, iy, iz);
//                            }
//                        } /* check if enter layer */
//                        else
//                        { /* check if exit layer */
//                            /* NOTE: this may not handle if passes thru edge voxel */
//                            s[0] = 0;
//                            if ((this_z > MIN_Z) && (next_z <= MIN_Z))
//                                s[0] = (MIN_Z - this_z) / (next_z - this_z);
//                            else if ((this_z < MAX_Z) && (next_z > MAX_Z))
//                                s[0] = (MAX_Z - this_z) / (next_z - this_z);
//                            /* determine point of intersection */
//                            if ((s[0] > 0) && (s[0] <= 1))
//                            {  /* crossed out of layer */
//                                xmid = this_x + s[0] * (next_x - this_x);
//                                ymid = this_y + s[0] * (next_y - this_y);
//                                zmid = this_z + s[0] * (next_z - this_z);
//                                ix = (int)Math.Floor((xmid - MIN_X) / dx);
//                                iy = (int)Math.Floor((ymid - MIN_Y) / dy);
//                                if (next_z > this_z)
//                                { /* exit bottom */
//                                    iz = nz - 1;
//                                    side = 5;
//                                    mu = (next_z - this_z) / tracklen;
//                                    if (in_layer)
//                                    {
//                                        if (Math.Abs(mu) > MU_LB)
//                                            outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / mu;
//                                        else
//                                            outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / (MU_LB / 2);
//                                    }
//                                }
//                                else
//                                { /* exit top */
//                                    iz = 0;
//                                    side = 0;
//                                    mu = -(next_z - this_z) / tracklen;
//                                    if (in_layer)
//                                    {
//                                        if (Math.Abs(mu) > MU_LB)
//                                            outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / mu;
//                                        else
//                                            outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / (MU_LB / 2);
//                                    }
//                                }
//                                in_layer = false;
//                                if (debug)
//                                {
//                                    Console.WriteLine("out[side={0},{1},{2},{3}]={4}", side, ix, iy, iz,
//                                        outptr.out_side_allvox[side][ix, iy, iz]);
//                                    Console.WriteLine("exit layer at (x,y,z)=({0},{1},{2})", xmid, ymid, zmid);
//                                    Console.WriteLine("exit: dist_in_vox(%{0},{1},{2},)={3}", ix, iy, iz,
//                                            Math.Sqrt((this_x - xmid) * (this_x - xmid) +
//                                                 (this_y - ymid) * (this_y - ymid) +
//                                                 (this_z - zmid) * (this_z - zmid)));
//                                }
//                            } /* crossed out of layer */
//                        } /* check if exit layer */
//                        while ((!next_same_vox) && (in_layer) && (!dead))
//                        {
//                            /* if boundary upward boundary collision first segment, save exiting wt */
//                            if ((photptr.histptr.boundary_col[ktrk] == 1) &&
//                                (this_z > next_z) && (this_z == zmid))
//                            {
//                                if (Math.Abs(mu) > MU_LB)
//                                    outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / mu;
//                                else
//                                    outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / (MU_LB / 2);
//                                if (debug)
//                                    Console.WriteLine("out[side={0},{1},{2},{3}]={4}", side, ix, iy, iz,
//                                        outptr.out_side_allvox[side][ix, iy, iz]);
//                                --iz;
//                            }
//                            /* check if ended in this voxel */
//                            if (debug) Console.WriteLine("floor((next_x-MIN_X)/dx)={0} ix={1}\n",
//                                           Math.Floor((next_x - MIN_X) / dx), ix);
//                            if (debug) Console.WriteLine("floor((next_y-MIN_Y)/dy)={0}f iy={1}\n",
//                                           Math.Floor((next_y - MIN_Y) / dy), iy);
//                            if (debug) Console.WriteLine("floor((next_z-MIN_Z)/dz)={0}f iz={1}\n",
//                                           Math.Floor((next_z - MIN_Z) / dz), iz);
//                            if ((Math.Floor((next_x - MIN_X) / dx) == (double)ix) &&
//                             (Math.Floor((next_y - MIN_Y) / dy) == (double)iy) &&
//                             (Math.Floor((next_z - MIN_Z) / dz) == (double)iz))
//                            { /* floor fix */
//                                next_same_vox = true;
//                                if (debug)
//                                {
//                                    Console.WriteLine("while:track ended in vox");
//                                    Console.WriteLine("while:dist_in_vox({0},{1},{2})={3}", ix, iy, iz,
//                                            Math.Sqrt((next_x - xmid) * (next_x - xmid) +
//                                                 (next_y - ymid) * (next_y - ymid) +
//                                                 (next_z - zmid) * (next_z - zmid)));
//                                }
//                            } /* track ended in vox */
//                            else
//                            { /* check which face track exited */
//                                /* i of s[i] based on 0=top,1=front,2=left,3=back,4=right,5=bot */
//                                if (debug) Console.WriteLine("fmod(xmid-MIN_X,dx)={0} y={1} z={2}\n",
//                                (xmid - MIN_X) % dx, (ymid - MIN_Y) % dy, (zmid - MIN_Z) % dz);
//                                if ((((xmid - MIN_X) % dx) < 1e-10) || /* on y-z face */
//                                  (Math.Abs(((xmid - MIN_X) % dx) - dx) < 1e-10))
//                                {
//                                    if (debug) Console.WriteLine("on y-z face");
//                                    if (next_x > this_x)
//                                    {
//                                        s[2] = 99;
//                                        s[4] = (MIN_X + (ix + 1) * dx - xmid) / (next_x - xmid);
//                                    }
//                                    else
//                                    {
//                                        s[2] = (MIN_X + (ix) * dx - xmid) / (next_x - xmid);
//                                        s[4] = 99;
//                                    }
//                                    s[3] = (MIN_Y + iy * dy - ymid) / (next_y - ymid);
//                                    s[1] = (MIN_Y + (iy + 1) * dy - ymid) / (next_y - ymid);
//                                    s[0] = (MIN_Z + iz * dz - zmid) / (next_z - zmid);
//                                    s[5] = (MIN_Z + (iz + 1) * dz - zmid) / (next_z - zmid);
//                                }
//                                else if ((((ymid - MIN_Y) % dy) < 1e-10) ||  /* on x-z face */
//                                    (Math.Abs(((ymid - MIN_Y) % dy) - dy) < 1e-10))
//                                {
//                                    if (debug) Console.WriteLine("on x-z face");
//                                    s[2] = (MIN_X + ix * dx - xmid) / (next_x - xmid);
//                                    s[4] = (MIN_X + (ix + 1) * dx - xmid) / (next_x - xmid);
//                                    if (next_y > this_y)
//                                    {
//                                        s[3] = 99;
//                                        s[1] = (MIN_Y + (iy + 1) * dy - ymid) / (next_y - ymid);
//                                    }
//                                    else
//                                    {
//                                        s[3] = (MIN_Y + (iy) * dy - ymid) / (next_y - ymid);
//                                        s[1] = 99;
//                                    }
//                                    s[0] = (MIN_Z + iz * dz - zmid) / (next_z - zmid);
//                                    s[5] = (MIN_Z + (iz + 1) * dz - zmid) / (next_z - zmid);
//                                }
//                                else if ((((zmid - MIN_Z) % dz) < 1e-10) ||  /* on x-y face */
//                                    (Math.Abs(((zmid - MIN_Z) % dz) - dz) < 1e-10))
//                                {
//                                    if (debug) Console.WriteLine("on x-y face");
//                                    s[2] = (MIN_X + ix * dx - xmid) / (next_x - xmid);
//                                    s[4] = (MIN_X + (ix + 1) * dx - xmid) / (next_x - xmid);
//                                    s[3] = (MIN_Y + iy * dy - ymid) / (next_y - ymid);
//                                    s[1] = (MIN_Y + (iy + 1) * dy - ymid) / (next_y - ymid);
//                                    if (next_z > this_z)
//                                    {
//                                        s[0] = 99;
//                                        s[5] = (MIN_Z + (iz + 1) * dz - zmid) / (next_z - zmid);
//                                    }
//                                    else
//                                    {
//                                        s[0] = (MIN_Z + (iz) * dz - zmid) / (next_z - zmid);
//                                        s[5] = 99;
//                                    }
//                                }
//                                else
//                                { /* this interior to voxel */
//                                    if (debug) Console.WriteLine("interior to voxel");
//                                    s[2] = (MIN_X + ix * dx - xmid) / (next_x - xmid);
//                                    s[4] = (MIN_X + (ix + 1) * dx - xmid) / (next_x - xmid);
//                                    s[3] = (MIN_Y + iy * dy - ymid) / (next_y - ymid);
//                                    s[1] = (MIN_Y + (iy + 1) * dy - ymid) / (next_y - ymid);
//                                    s[0] = (MIN_Z + (iz) * dz - zmid) / (next_z - zmid);
//                                    s[5] = (MIN_Z + (iz + 1) * dz - zmid) / (next_z - zmid);
//                                }
//                                mins = 99;
//                                jfix = -1;
//                                for (j = 0; j < 6; ++j)
//                                {
//                                    if ((s[j] > 0) && (s[j] <= 1 + 1e-10))
//                                    { /* hit plane */
//                                        if (Math.Abs(s[j]) > 1e-10)
//                                        {
//                                            if (s[j] <= mins)
//                                            {
//                                                mins = s[j];
//                                                jfix = j;
//                                            }
//                                        }
//                                    }
//                                }
//                                if ((mins == 99) && (photptr.histptr.boundary_col[ktrk] == 0))
//                                {
//                                    Console.WriteLine("WARNING: N={0} trk={1} mins==99 x,y,z={2},{3},{4}",
//                                 bananaptr.banana_photons, ktrk, this_x, this_y, this_z);
//                                    dead = true;
//                                }
//                                side = jfix;
//                                switch (jfix)
//                                {
//                                    case 0: mu = -(next_z - this_z) / tracklen; break;
//                                    case 1: mu = (next_y - this_y) / tracklen; break;
//                                    case 2: mu = -(next_x - this_x) / tracklen; break;
//                                    case 3: mu = -(next_y - this_y) / tracklen; break;
//                                    case 4: mu = (next_x - this_x) / tracklen; break;
//                                    case 5: mu = (next_z - this_z) / tracklen; break;
//                                }
//                                xmid2 = xmid + mins * (next_x - xmid);
//                                ymid2 = ymid + mins * (next_y - ymid);
//                                zmid2 = zmid + mins * (next_z - zmid);
//                                if (debug)
//                                {
//                                    Console.WriteLine("jfix=%d s=({0},{1},{2},{3},{4},{5})", jfix, s[0], s[1], s[2], s[3], s[4], s[5]);
//                                    Console.WriteLine("intra layer at (x,y,z)=({0},{1},{2})\n", xmid2, ymid2, zmid2);
//                                    Console.WriteLine("intra:dist_in_vox({0},{1},{2})={3}\n", ix, iy, iz,
//                                            Math.Sqrt((xmid2 - xmid) * (xmid2 - xmid) +
//                                                 (ymid2 - ymid) * (ymid2 - ymid) +
//                                                 (zmid2 - zmid) * (zmid2 - zmid)));
//                                }
//                                /* save weights since exiting current voxel */
//                                if ((ix >= nx) || (ix < 0) || (iy >= ny) || (iy < 0) || (iz >= nz) || (iz < 0))
//                                    in_layer = false;
//                                if (in_layer)
//                                {
//                                    if (Math.Abs(mu) > MU_LB)
//                                        outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / mu;
//                                    else
//                                        outptr.out_side_allvox[side][ix, iy, iz] += phot_disc_wt / (MU_LB / 2);
//                                }
//                                if (debug) Console.WriteLine("out[side={0},{1},{2},{3}]={4}", side, ix, iy, iz,
//                                        outptr.out_side_allvox[side][ix, iy, iz]);
//                                /* move to the next voxel */
//                                switch (jfix)
//                                {
//                                    case 0: --iz; if (!exit) side = 5; break;
//                                    case 1: ++iy; if (!exit) side = 3; break;
//                                    case 2: --ix; if (!exit) side = 4; break;
//                                    case 3: --iy; if (!exit) side = 1; break;
//                                    case 4: ++ix; if (!exit) side = 2; break;
//                                    case 5: ++iz; if (!exit) side = 0; break;
//                                }
//                                if (debug) Console.WriteLine("next (ix,iy,iz)=({0},{1},{2})", ix, iy, iz);
//                                if ((ix >= nx) || (ix < 0) || (iy >= ny) || (iy < 0) || (iz >= nz) || (iz < 0))
//                                    in_layer = false;
//                                /* adjoint: save weights since entering voxel */
//                                if (in_layer)
//                                {
//                                    outptr.in_side_allvox[side][ix, iy, iz] += phot_disc_wt;
//                                }
//                                xmid = xmid2;
//                                ymid = ymid2;
//                                zmid = zmid2;
//                            } /* else crossed voxel face */
//                        } /* while not at end of track or dead */
//                    } /* not out sides */
//                    /* only deweight non-boundary collisions */
//                    if ((photptr.histptr.boundary_col[ktrk] == 0) || (ktrk == 0))
//                    {
//                        curr_layer = 0;
//                        for (i = 1; i <= tissptr.num_layers; i++)
//                        {
//                            if ((this_z >= Tissue.Regions[i].zbegin) && /* must >= */
//                                 (this_z <= Tissue.Regions[i].zend))
//                                curr_layer = i;
//                        }
//                        phot_disc_wt *= Tissue.Regions[curr_layer].mus /
//                            (Tissue.Regions[curr_layer].mus +
//                             Tissue.Regions[curr_layer].mua);
//                    } /* if not boundary collision */
//                    else
//                        ++bdry_col;
//                } /* if not dead */
//            } /* for all tracks */
//        }
//        ///**************************************************************/
//        //void Add_To_List(vox_list list, 
//        //                int ix, int iy, int iz, int col, int sa_idx)
//        //{
//        //  vox_list temp = new vox_list();
//        //  temp.ix=ix;
//        //  temp.iy=iy;
//        //  temp.iz=iz;
//        //  temp.col=col;
//        //  temp.sa_idx=sa_idx;
//        //  temp.next=*this_vox;
//        //  voxelLinkedList[
//        //  /* printf("Add end: *this_vox=%d ix,iy,iz=%d,%d,%d\n",
//        //    *this_vox,ix,iy,iz);  */
//        //}
//        ///************************************************************/
//        //void Free_List(struct vox_list *head)
//        //{
//        //  struct vox_list *temp;
//        //  /* printf("Free_List: head=%d\n",head);  */
//        //  while (head!=NULL) {
//        //    temp=head.next;
//        //    free(head);
//        //    head=temp;
//        //  }
//        //}
//        /************************************************************/
//        void Compute_Wts_allvox()
//        {
//            //FILE *ofp[6];
//            int iw, num_sides = 6;
//            long N;
//            //string tmp[256];
//            /* QFIX: assume symmetry . use forward for adjoint */
//            double src_NA = photptr.src_NA, det_NA = src_NA;
//            double dx, dy, dz, delmu, delphi, Rhoog_norm;
//            double Asrc, Adet, n = Tissue.Regions[1].n;
//            /* delmu=1.0/bananaptr.num_mu;
//            delphi=2*PI/bananaptr.num_phi; */
//            delmu = 1.0; /* assume 1 angular bin now */
//            delphi = 2 * PI;
//            dx = tissptr.dx;
//            dy = tissptr.dr;
//            dz = tissptr.dz;
//            N = (long)tissptr.num_photons;
//            /* NOTE: norm/denom(dx*dy*dz*dmu*dphi) factor of adjoint files */

//            // save outptr.out_side_allvox[iw, ix, iy, iz]...with name: "wts_out_side" + iw

//            if (source.beam_radius == 0.0)
//                Asrc = 1;
//            else
//                Asrc = PI * source.beam_radius * source.beam_radius;
//            /* Adet=PI*pertptr.det_rad*pertptr.det_rad; */
//            Adet = Asrc;
//            Rhoog_norm = Asrc * 2 * PI * (1 - Math.Sqrt(1 - (src_NA / n) * (src_NA / n))) *
//                   Adet * 2 * PI * (1 - Math.Sqrt(1 - (det_NA / n) * (det_NA / n)));

//            //Console.WriteLine("in_side[0][200][0][0]={0}", outptr.in_side_allvox[0][200, 0, 0]);

//            for (iw = 0; iw < num_sides; ++iw)
//            {
//                double area;
//                switch (iw)
//                {
//                    case 0:
//                    case 5:
//                        area = dx * dy;
//                        break;
//                    case 1:
//                    case 3:
//                        area = dx * dz;
//                        break;
//                    case 2:
//                    case 4:
//                        area = dy * dz;
//                        break;
//                }

//                // todo: fix arrays to be either 1D or 1D jagged
//                for (int k = 0; k < outptr.in_side_allvox[iw].GetLength(2); k++)
//                    for (int j = 0; j < outptr.in_side_allvox[iw].GetLength(1); j++)
//                        for (int i = 0; i < outptr.in_side_allvox[iw].GetLength(0); i++)
//                            outptr.in_side_allvox[iw][i,j,k] *=
//                                Rhoog_norm / (delmu * delphi * dy * dz * N * N);
//            } /* for iw */
//        }
//    }
//}
