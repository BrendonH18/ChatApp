import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faSearch } from "@fortawesome/free-solid-svg-icons"
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"



const Channels = ({ connection, channel, availableChannels, ActiveChannelID, isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
    const [search, setSearch] = useState("")
    const [trimChannels, setTrimChannels] = useState(availableChannels)
    let navigate = useNavigate()

    const trimAvailableChannels = () => {
        if (search === "") return setTrimChannels(availableChannels)
        setTrimChannels( availableChannels.filter(ch => ch.name.toLowerCase().includes(search.toLowerCase())))
    }

    const handleChannelSelect = (channel) => {
        navigate(`/Channel/${channel.id}`)
    }

    useEffect(trimAvailableChannels,[search, availableChannels])

    useEffect(() => {
        if(!availableChannels) return
        if(!connection) return
        let id = 1;
        if(parseInt(ActiveChannelID) !== 0 || typeof ActiveChannelID !== "undefined") id = parseInt(ActiveChannelID)
        if(id === parseInt(channel.id)) return
        connection.send("JoinChannel", availableChannels.find(x=> x.id === id))
      }, [ActiveChannelID, availableChannels])
    
    return <>
    <div className="card mb-sm-3 mb-md-0 contacts_card">
        <div className="card-header">
            <div className="input-group">
                <input type="text" placeholder="Search..." name="" className="form-control search" onChange={e => setSearch(e.target.value)} value={search}/>
                <div className="input-group-prepend">
                    <span className="input-group-text search_btn h-100">
                        <FontAwesomeIcon icon={faSearch}/>
                    </span>
                </div>
            </div>
        </div>
        <div className="card-body contacts_body">
            <ul className="contacts">
                {trimChannels
                    ? trimChannels.map(channel => 
                        <li key={`channel-${channel.id}`} className={parseInt(ActiveChannelID)===parseInt(channel.id) ? "active" : ""} onClick={e => handleChannelSelect(channel)}>
                            <div className="d-flex bd-highlight" >
                                <div className="img_cont" >
                                    <img src={channel.image
                                        ? channel.image
                                        : "https://static.turbosquid.com/Preview/001292/481/WV/_D.jpg"} className="rounded-circle user_img" />
                                    {isConnectedUsersByChannelAndStatusTruthy()
                                        ? <span className="online_icon offline"></span>
                                        // ? <span className={connectedUsersByChannelAndStatus[channel.id]["Active"].length>0 ? "online_icon" : "online_icon offline"} ></span>
                                        : <></>}
                                </div>
                                <div className="user_info" >
                                    <span >{channel.name}</span>
                                    {isConnectedUsersByChannelAndStatusTruthy()
                                        ? <p> XX Connected</p>
                                        // ? <p>{`${connectedUsersByChannelAndStatus[channel.id]["Active"].length} Connected`}</p>
                                        : <></>}
                                </div>
                            </div>
                        </li>
                    )
                    : <></>}
            </ul>
        </div>
        <div className="card-footer"></div>
    </div>
</>
}

export default Channels