<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs"
Inherits="Install_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>MakerShop {0} License Agreement</title>
    <style>
      p.license {
        font-size: 14px;
        font-family: monospace;
        text-align: justify;
        margin-bottom: 12px;
      }
      .sectionHeader {
        background-color: #efefef;
        padding: 3px;
        margin: 12px 0px;
      }
      h1 {
        font-size: 18px;
        font-weight: bold;
        margin: 8px 0px;
      }
      h1.warning {
        margin-bottom: 12px;
      }
      h2 {
        font-size: 14px;
        font-weight: bold;
        margin: 0px;
      }
      h3 {
        font-size: 14px;
        font-weight: bold;
        margin: 0px;
        margin-top: 6px;
      }
      .error {
        font-weight: bold;
        color: red;
      }
      div.radio {
        margin: 2px 0px 6px 0px;
      }
      div.radio label {
        font-weight: bold;
        padding-top: 6px;
        position: relative;
        top: 1px;
      }
      .inputBox {
        padding: 6px;
        margin: 4px 40px;
        border: solid 1px #cccccc;
      }
      div.submit {
        background-color: #efefef;
        padding: 4px;
        margin: 12px 0px;
        text-align: center;
      }
    </style>
    <script type="text/javascript" language="JavaScript">
      var counter = 0;
      function plswt() {
        counter++;
        if (counter > 1) {
          alert(
            "You have already submitted this form.  Please wait while the install processes."
          );
          return false;
        }
        return true;
      }
    </script>
  </head>
  <body style="width: 780px; margin: auto">
    <form id="form1" runat="server">
      <asp:PlaceHolder
        ID="phAgreement"
        runat="server"
        EnableViewState="false"
        Visible="true"
      >
        <h1>
          <span runat="server" Id="Heading"
            >MakerShop {0} License Agreement</span
          ><span class="noPrint"
            >&nbsp;&nbsp;&nbsp;<asp:LinkButton
              ID="PrintButton"
              runat="server"
              Text="PRINT"
              OnClientClick="window.print();return false;"
              SkinID="Button"
          /></span>
        </h1>
        <p class="license">
          BY CLICKING ON THE "I HAVE READ AND AGREE TO THE ABOVE TERMS OF THIS
          LICENSE AGREEMENT. I HAVE AUTHORITY TO EXECUTE THIS LICENSE AGREEMENT"
          BUTTON BELOW, YOU ARE CREATING A LEGAL AND BINDING AGREEMENT
          ("LICENSE" HEREIN) BETWEEN YOU OR A COMPANY FOR WHOM YOU ACT
          ("LICENSEE" HEREIN) AND ABLE SOLUTIONS CORPORATION FOR ITS DIVISION
          MakerShop, ("OWNER" HEREIN)
        </p>
        <p class="license">
          1. Owner hereby licenses to Licensee the following non-exclusive,
          non-transferable rights in and to that certain software known as
          MakerShop ("The Application" herein) but only for use according to the
          terms agreed to by Licensee at the time of sign up for this license,
          which terms are "1 store to be operated at 1 domain" ("Licensed Use"
          herein):
        </p>
        <p class="license">
          a. the right to use the licensed copy of The Application in executable
          form for internal operations on a computer;
        </p>
        <p class="license">
          b. the right to use and copy The Application user�s guide for
          Licensee�s internal operations;
        </p>
        <p class="license">
          c. the right to make backup copies of The Application for the limited
          purpose of safety and security but such right shall not allow Licensee
          to transfer or allow the transfer of copies of The Application, to use
          The Application on more than a single computer or exceed the maximum
          number of store licenses delivered to Licensee, to install or use
          copies of The Application on more computers than the Licensed Use.
        </p>
        <p class="license">
          Except as expressly licensed to Licensee as set forth in paragraph 1
          above, Licensee shall have no other rights in or to The Application or
          any elements of The Application and this License shall be interpreted
          narrowly. Without limiting the generality of the foregoing, Licensee
          shall not be entitled to prepare or allow others to prepare or assist
          in the preparation of any software or code that is similar to The
          Application or reproduce, modify, decompile, reverse engineer,
          disassemble or reduce The Application to readable form.
        </p>
        <p class="license">
          2. Owner is and shall be the sole and exclusive owner of all rights of
          every kind, nature and description including but not limited to all
          rights of contract, patent, copyright, trademark, trade dress and all
          other rights, throughout the world, during the terms of the copyright
          including all renewals and extensions thereof of The Application and
          in perpetuity, in and to The Application and in and to any derivative
          works based on The Application, free of any claims of any sort on the
          part of Licensee and including but not limited to all rights of
          reproduction and all of the foregoing in all languages, in all media
          and forms of expression both electronically and non-electronically and
          otherwise, whether now existing or hereafter discovered, and all of
          the foregoing throughout the world and including but not limited to
          all such rights in and to the title or titles of The Application, the
          exclusive right to market, sell, transfer, license and otherwise deal
          in and with The Application and the exclusive right to sell, assign,
          transfer, license and otherwise dispose of any or all of Owner�s
          rights to others including but not limited to any corporations,
          limited liability companies or other entities owned and controlled by
          or related to Owner and to grant such others the right to do so. All
          of the rights granted to the Owner herein shall remain valid and
          effective as to all of The Application even if Owner does not exploit
          or use any particular right or rights. Each and every of Owner�s
          rights shall apply to any revised edition or revisions of The
          Application.
        </p>
        <p class="license">3. Licensee represents and warrants:</p>
        <p class="license">
          a. That Licensee has the legal right and authority to enter into and
          perform Licensee�s obligations under this License; and
        </p>
        <p class="license">
          b. That the execution and performance of this License will not
          conflict with or violate any provision of any law having applicability
          to the within transaction and will not violate the rights of any other
          parties including but not limited to any trade secrets, confidential
          information, contract, patent, copyright, trademark, trade dress or
          any other rights.
        </p>
        <p class="license">
          These warranties shall survive the termination of this License.
        </p>
        <p class="license">
          4. Licensee shall defend and hold harmless Owner, Owner�s affiliated
          and related business entities, and all of their or Owner�s employees,
          officers, directors, members, shareholders, agents, attorneys,
          successors and assigns and all other parties against any and all
          claims, demands, costs, awards, damages and the like, including
          attorneys fees, that may arise from a breach or claimed breach of any
          of Licensee�s warranties, representations or obligations in this
          License, whether or not a breach of those warranties, representations
          or obligations is finally sustained and whether or not any litigation
          or claim is filed. If a claim, action, or proceeding is brought
          against Owner or any of the parties above that would violate any of
          Licensee�s warranties, representations or obligations, Owner may
          choose to respond to or defend the same through counsel Owner chooses
          or have Licensee engage attorneys to do the same and Owner may settle
          any such claim and the like in Owner�s sole discretion and Author
          shall pay for all such costs and damages including settlement costs.
          These indemnities shall survive the termination of this agreement.
        </p>
        <p class="license">
          5. All aspects of The Application are owned exclusively by Owner and
          contain confidential information. In addition to all other
          representations and warranties of Licensee in this License, Licensee
          represents and warrants that: (i) Licensee shall limit access to The
          Application to Licensee�s bona fide employees who have a need to
          access The Application for the purposes of exercising Licensee�s
          rights under this License; (ii) Licensee and any other permitted
          parties shall keep The Application in confidence and shall not use The
          Application to benefit Licensee or any other parties in any manner
          whatsoever; (iii) The Application shall be used only for the purpose
          of Licensee�s own internal use and neither Licensee nor anyone in
          Licensee�s company or affiliated with Licensee nor any other parties
          will use The Application or disclose The Application unless and until
          expressly authorized in writing to do so by Owner; (iv) None of The
          Application shall be copied or reproduced by Licensee or any other
          party without the express, prior, written permission of Owner, except
          for a single back up copy to be used only for security purposes; (v)
          The Application and all copies thereof shall be returned intact to
          Owner within one (1) day of a request by Owner for the return of same
          or within one (1) day after the termination or expiration of this
          License even if no such written request is made, whichever shall be
          the sooner, and that such return shall constitute a warranty and
          representation by Licensee that all copies have been returned; (vi)
          Neither Licensee nor anyone else shall alter or destroy any of The
          Application except as set forth specifically in this paragraph; (vii)
          Licensee will be responsible for any breach or claimed breach of the
          terms of the provisions of this paragraph and this License by any of
          Licensee�s employees, consultants or other parties who receive The
          Application; (viii) Licensee shall use all measures to maintain the
          security and confidentiality of The Application including securing all
          networks and other channels whereby the same may be accessed; and (ix)
          Licensee shall have no right to assign, delegate, transfer or
          otherwise dispose of any of Licensee�s rights or obligations under
          this License.
        </p>
        <p class="license">
          a. Licensee hereby expressly agrees that all of The Information is
          confidential and proprietary, the loss or disclosure of which would
          cause Owner great and irreparable injury, damage and harm which could
          not be adequately compensated for in damages in an action at law. The
          within remedies shall be in addition to any other remedies otherwise
          available to Owner. In the event that Owner claims that the ability to
          calculate damages in the event of Licensee�s breach of this agreement
          would be difficult, Licensee agrees that Owner shall be entitled to
          liquidated damages of $10,000.00 for each breach hereof.
        </p>
        <p class="license">
          b. This License shall be construed and interpreted to mean that Owner
          shall have the broader of such rights and remedies under either this
          License, common law or the Uniform Trade Secrets Act or any version of
          the Uniform Trade Secrets Act under the laws of state of Washington,
          any other state or any other laws.
        </p>
        <p class="license">
          c. All of the foregoing provisions shall apply both during and after
          the term of this License.
        </p>
        <p class="license">
          8. THE APPLICATION IS PROVIDED ON AN "AS IS" BASIS WITHOUT ANY
          WARRANTIES, EXPRESS OR IMPLIED, OF ANY KIND WHETHER AS TO USE,
          MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE OR OTHERWISE AND
          WHETHER ARISING UNDER LAW OR FROM COURSE OF PERFORMANCE, COURSE OF
          DEALING, OR USAGE OF TRADE OR OTHERWISE. WITHOUT LIMITING THE
          GENERALITY OF THE FOREGOING, OWNER DOES NOT WARRANT THAT THE
          APPLICATION SHALL BE SUCCESSFUL IN CREATING ANY SALES OR INCOME FOR
          LICENSEE. ALL SUCH WARRANTIES ARE HEREBY EXPRESSLY EXCLUDED BY OWNER
          AND FOREVER WAIVED BY LICENSEE. NO STATEMENTS, INFORMATION OR OTHER
          COMMUNICATIONS GIVEN BY OWNER OR ANY OTHER PARTIES SHALL WAIVE ANY OF
          THE WITHIN EXCLUSIONS. OWNER SHALL NOT BE LIABLE TO LICENSEE OR ANY
          OTHER PARTY FOR ANY DIRECT OR INDIRECT COMPENSATORY, SPECIAL,
          INCIDENTAL, OR CONSEQUENTIAL DAMAGES OR COSTS OF ANY CHARACTER
          INCLUDING BUT NOT LIMITED TO DAMAGES OR COSTS FOR COPYRIGHT
          INFRINGEMENT, TRADEMARK INFRINGEMENT, PATENT INFRINGEMENT, LIBEL,
          SLANDER, INVASION OF PRIVACY, INTERFERENCE WITH THE RIGHTS OF
          PUBLICITY, LOSS OF GOODWILL, LOSS OF DATA, LOST PROFITS, WORK
          STOPPAGE, COMPUTER FAILURE OR MALFUNCTION WHETHER AS A RESULT OF
          VIRUSES, TROJAN HORSES OR OTHERWISE, OR FOR FAILURE OF ANY PARTY TO
          PROVIDE INTERNET ACCESS FOR ANY PERIOD OF TIME OR FOR THE ACTS OR
          SOFTWARE OR PROGRAMS OF ANY OTHER PARTY INCLUDING BUT NOT LIMITED TO
          "HACKERS" OR OTHERS OR FOR ANY AND ALL OTHER DAMAGES OR COSTS
          INCLUDING PUNITIVE DAMAGES OR LOSSES. LICENSEE ASSUMES TOTAL
          RESPONSIBILITY FOR THE USE OF THE APPLICATION. OWNER WILL NOT BE
          LIABLE FOR ANY DAMAGES OR COSTS IN EXCESS OF THE TOTAL CONTRACT PRICE
          FOR THE LICENSE HEREIN EVEN IF OWNER SHALL HAVE BEEN INFORMED OF THE
          POSSIBILITY OF SUCH DAMAGES OR COSTS, OR FOR ANY CLAIM BY ANY OTHER
          PARTY. THE WITHIN LIMITATION OF WARRANTIES MAY BE LIMITED BY THE LAWS
          OF CERTAIN STATES OR OTHER JURISDICTIONS AND SO SOME OF THE FOREGOING
          LIMITATIONS MAY NOT APPLY TO LICENSEE AND LICENSEE MAY HAVE OTHER
          RIGHTS THAT MAY VARY FROM STATE TO STATE. IN THE EVENT THAT ANY OF THE
          WARRANTIES CANNOT BE DISCLAIMED OR WAIVED UNDER APPLICABLE LAW,
          LICENSEE�S SOLE AND EXCLUSIVE REMEDY FOR BREACH OF SUCH DISCLAIMED
          WARRANTY WILL BE, AT OWNER�S SOLE DISCRETION, EITHER (i) THE
          REPLACEMENT OF THE APPLICATION, AT NO ADDITIONAL COST TO LICENSEE, OR
          (ii) THE REFUND OF ANY LICENSE FEES ACTUALLY PAID BY LICENSEE UNDER
          THIS AGREEMENT.
        </p>
        <p class="license">
          9. The term of this License shall commence on the date this License is
          fully signed and shall continue as long as Licensee is using The
          Application or until the earlier termination of this License pursuant
          to the provisions set forth herein.
        </p>
        <p class="license">
          a. Licensee shall be deemed in breach of this License if License does
          or if Owner claims that Licensee has done any of the following: (i)
          Fails to pay any amount due under this License; (ii) Fails to cure a
          breach or claim of breach by Licensee within ten (10) days of Owner�s
          notification to Licensee but only if such breach or claimed breach is
          curable but the foregoing shall not apply to any failure on the part
          of Licensee to pay any amount under this License; or (iii) Licensee
          sells, transfers or assigns or enters into any agreement to sell,
          transfer or assign all or any part of Licensee�s business. The
          foregoing enumeration of breaches or claimed breaches shall not be in
          limitation of any other basis for such breaches or claimed breaches.
        </p>
        <p class="license">
          b. In the event of any of the bases of breach or claimed breach arise
          as set forth in paragraph 9. a. above, Owner shall have the right, but
          not the obligation, to terminate this License. In the event of such
          termination, Licensee shall pay to Owner all costs and expenses
          (including reasonable legal fees and costs and fees of collection
          agencies, if any) incurred by Owner in connection with such
          termination. The foregoing right and remedy on the part of Owner shall
          not be in limitation of any other rights or remedies available to
          Owner.
        </p>
        <p class="license">
          c. Upon termination or expiration of this License for any reason
          whatsoever, Licensee shall immediately: (i) cease all use of The
          Application; and (ii) return or destroy and certify such destruction,
          at Owner�s discretion, The Application and shall remove all copies of
          all of the same from Licensee computers and storage media.
        </p>
        <p class="license">
          10. Licensee shall pay to Owner the fees set forth on Exhibit "A,"
          which Exhibit "A" is attached to and made a part of this License. All
          payments shall be made in U.S. currency, online payment, credit card
          payment or wire transfer, at Owner�s election, and any cost of
          conversion, collection, bank costs (for either Licensee�s bank or
          Owner�s bank) shall be borne solely by Licensee. Licensee shall pay a
          late fee of 1.5 % per month on any amount not paid when due. In
          addition to fees to Owner, Licensee shall pay any amounts due for
          taxes, duties, import or export fees or other fees payable to other
          parties including those paid on behalf of Licensee by Owner including
          but not limited to governments.
        </p>
        <p class="license">
          11. Licensee shall not be entitled to assign this License or transfer
          any of Licensee�s rights or delegate any of Licensee�s obligations
          without the prior, express written consent of Owner. Owner may assign
          all or any part of this License or delegate all or any part of Owner�s
          obligations without the prior written consent of Licensee to any other
          party including but not limited to any party owned by or related to
          Owner.
        </p>
        <p class="license">
          12. The Application and any documentation and all related technical
          information or materials may be subject to export controls and
          licensable under the U.S. Government export regulations. Licensee
          shall comply strictly with all legal requirements established under
          these controls and shall not import, export, re-export, divert,
          transfer or disclose, directly or indirectly, The Application,
          documentation and any related technical information or materials
          without the prior approval of the U.S. Department of Commerce if
          applicable.
        </p>
        <p class="license">
          13. Owner and Licensee are independent contractors with regards one
          another and are not partners, joint venturers, employer-employee or
          any other relationship.
        </p>
        <p class="license">
          14. Any dispute arising under this License shall be shall be submitted
          for binding arbitration under the Commercial Arbitration Rules of the
          American Arbitration Association ("AAA" herein) and Owner shall be
          entitled to injunctive relief and shall be conducted in confidence and
          shall not be made public. The arbitration shall be heard by a panel of
          three (3) arbitrators selected by the AAA and Owner and Licensee.
          Judgment on any award entered therein may be entered in any court of
          competent jurisdiction. The arbitration shall be in Vancouver,
          Washington USA. Licensee expressly consents to personal jurisdiction
          in the AAA and in such courts. In all cases, each party shall
          initially bear its own costs relating to such arbitration and the
          parties shall initially equally share the arbitrators� fees except
          that the prevailing party shall be entitled to its attorneys fees and
          costs or otherwise determined by the arbitrator.
        </p>
        <p class="license">
          15. This License shall be subject to and interpreted under the laws of
          the state of Washington in the United States applicable to agreements
          wholly to be performed therein as well as the copyright law or other
          law or laws of the United States, where applicable. It is, along with
          the attached Exhibit "A," the complete understanding between Owner and
          Licensee and may be modified and any of its provisions waived only by
          a writing signed by both Owner and Licensee. It shall bind and benefit
          Owner�s and Licensee�s heirs, assigns and successors in interest but
          any restrictions on assignment and transfer otherwise contained in
          this License shall otherwise apply. A waiver by Owner of any provision
          of this License, or any claimed breach thereof, shall not be deemed a
          waiver of any other provision or breach. All remedies provided Owner
          in this License are cumulative and the exercise by Owner of any remedy
          shall be without prejudice to Owner�s exercise of any other rights or
          remedies available to Owner. In any action under this License, the
          prevailing party shall be entitled to attorneys fees and court costs.
          In the event any portion of this License shall be held invalid or
          unenforceable it shall not affect the validity or enforceability of
          the rest of this License. This License may be executed in one or more
          counterparts, each of which shall be deemed an original and all of
          which, taken together, shall be construed as a single instrument. Fax
          and email signatures shall be valid as originals. All terms and
          provisions of this License that should by their nature survive the
          termination shall so survive.
        </p>
        <p class="license">
          Exhibit "A". Licensee shall pay the following fees at the following
          times: $995.00 at time of purchase or as specified on the MakerShop
          website at
          http://www.MakerShop.com/AspNet-Shopping-Cart-70-by-MakerShop-P156C20.aspx
        </p>
        <div class="noPrint" style="text-align: center">
          <br />
          &nbsp;&nbsp;<asp:LinkButton
            ID="AcceptButton"
            runat="server"
            Text="I HAVE READ AND AGREE TO THE ABOVE TERMS OF THIS LICENSE AGREEMENT.  I HAVE AUTHORITY TO EXECUTE THIS LICENSE AGREEMENT."
            OnClick="AcceptButton_Click"
            OnClientClick="this.innerHTML='Processing...';return plswt();"
            SkinID="Button"
          /><br /><br />
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder
        ID="phPermissions"
        runat="server"
        EnableViewState="false"
        Visible="false"
      >
        <br />
        <div class="pageHeader">
          <h1 style="font-size: 16px">Installation Permissions Test</h1>
        </div>
        Sufficient file permissions are not available for MakerShop to install
        or run properly. Ensure that the specified process identity has
        permission to the specified folders and files. All tests should pass
        before installation completes.<br /><br /> <b>Process Identity:</b>
        <asp:Literal ID="ProcessIdentity" runat="server"></asp:Literal
        ><br /><br /> <b>Test Results:</b><br />
        <asp:Literal ID="PermissionsTestResult" runat="server"></asp:Literal>
      </asp:PlaceHolder>
    </form>
  </body>
</html>
