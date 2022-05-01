import { Outlet, useNavigate, useParams } from "react-router-dom";

const ChannelDashboard = ({ availableChannels, setUserConnection, userConnection }) => {
  
let navigate = useNavigate()

const handleClick = (e) => {
  const values = userConnection
  values.channel = availableChannels[e.target.id - 1]
  console.log(values)
  setUserConnection(values)
  navigate(values.channel.name)
}

    return(
      <>
      {availableChannels.map(x => <button 
        id={x.id}
        onClick={handleClick}
        >
          
          {`${x.name}`}</button>)}
          <Outlet/>
      </>
    )
  }
  
  export default ChannelDashboard;