const BubbleLabel = ({image, label, subLabel, isOnline}) => {

return <>
    <div className="d-flex bd-highlight" >
        <div className="img_cont" >
            <img src={image} className="rounded-circle user_img" />
            <span className={isOnline ? "online_icon" : "online_icon offline"}></span>
        </div>
        <div className="user_info" >
            <span >{label}</span>
            <p>{subLabel}</p>
        </div>
    </div>
</>
}

export default BubbleLabel