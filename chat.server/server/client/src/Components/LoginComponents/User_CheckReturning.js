import { useParams } from "react-router-dom";

const User_CheckReturning = ({ connectedUsers }) => {
  
    let { LoginType } = useParams();

    return(
      <>
      <h2>Check Returning - {LoginType}</h2>
      </>
    )
  }
  
  export default User_CheckReturning;